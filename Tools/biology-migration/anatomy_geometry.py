"""Extract source groups/material metadata; Unity builds the actual assets and scene."""
import json,re,runpy
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
SOURCE=ROOT/'SourceAssets/BiologyLegacy'
PARSE=runpy.run_path(str(Path(__file__).with_name('cell_geometry.py')))['parse']
GROUPS=[('resp','Sitema Respiratorio'),('circ','SistemaCirculatório'),('dig','Sistema Digestorio'),('nerv','NervousSystem'),('skel','Skeleton'),('muscle','Muscles'),('renal','Sistema Renal'),('lymph','LymphaticSystem'),('repro','Sistema Reprodutivo'),('skin','Body')]
def main():
    index={}
    for meta in SOURCE.rglob('*.meta'):
        match=re.search(r'^guid: ([a-f0-9]{32})',meta.read_text(encoding='utf-8'),re.M)
        if match:index[match[1]]=Path(str(meta)[:-5])
    groups=[];parts={}
    for key,name in GROUPS:
        path=SOURCE/'Assets/_Project/Prefabs/Biologia/CorpoHumano'/f'{name}.prefab'
        parsed=PARSE(path);nodes=[n for n in parsed['nodes'] if n['mesh']]
        groups.append({'id':key,'sourcePrefab':path.relative_to(ROOT).as_posix(),'names':[n['name'] for n in nodes]})
        for node in nodes:
            parts[node['name']]={'name':node['name'],'materials':[m['guid'] for m in node['materials']]}
    # These original groups include anatomical context/misclassifications, corrected explicitly.
    byid={g['id']:g['names'] for g in groups}
    byid['resp'].remove('Bronchi_Vessels');byid['circ'].append('Bronchi_Vessels')
    for name in ['SpinalCord_Neck','SpinalCord_Torso']:
        byid['skel'].remove(name);byid['nerv'].append(name)
    for name in ['Adrenal_Gland_Left','Adrenal_Gland_Right']:byid['renal'].remove(name)
    byid['muscle'].remove('MaxillaryHighmoreSinus')
    byid['dig'].append('Pharynx')
    used={n for g in groups for n in g['names']}
    guids={g for n in used for g in parts[n]['materials']}
    materials=[];textures={}
    for guid in sorted(guids):
        path=index[guid];raw=path.read_text(encoding='utf-8')
        def number(key,default):
            m=re.search(r'^    - '+re.escape(key)+r': ([\d.eE+-]+)',raw,re.M)
            return float(m[1]) if m else default
        def texture(key,normal):
            m=re.search(r'^    - '+re.escape(key)+r':\n\s+m_Texture: \{fileID: [^,}]+, guid: ([a-f0-9]{32})',raw,re.M)
            if not m:return ''
            tid=m[1];source=index[tid]
            textures[tid]={'guid':tid,'source':source.relative_to(ROOT).as_posix(),'normal':normal}
            return tid
        color=re.search(r'^    - _BaseColor: \{r: ([^,]+), g: ([^,]+), b: ([^,]+), a: ([^}]+)',raw,re.M)
        materials.append({'guid':guid,'name':path.stem,'color':dict(zip(['r','g','b','a'],map(float,color.groups()))) if color else dict(r=1,g=1,b=1,a=1),'smoothness':number('_Smoothness',.28),'bumpScale':number('_BumpScale',1),'albedo':texture('_BaseMap',False) or texture('_MainTex',False),'normal':texture('_BumpMap',True)})
    result={'groups':groups,'parts':[parts[n] for n in sorted(used)],'materials':materials,'textures':list(textures.values()),'alignment':'Use canonical FBX node transforms, never the displaced prefab roots.','excluded':['Adrenal glands: endocrine, outside the nine-system scope.','Maxillary sinus: not a muscle.'],'corrections':['Spinal cord belongs to nervous system.','Bronchial vessels belong to circulatory system.','Pharynx is shared by respiratory/digestive models.']}
    out=ROOT/'Assets/_Project/Art/Biology/Anatomy/AnatomyGeometry.json';out.parent.mkdir(parents=True,exist_ok=True)
    out.write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf-8')
    print('Groups:',len(groups),'parts:',len(used),'materials:',len(materials),'textures:',len(textures))
if __name__=='__main__':main()
