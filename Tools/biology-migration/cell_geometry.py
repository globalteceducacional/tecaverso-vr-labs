"""Extract only authored geometry/transforms from legacy cell prefabs.

No Unity YAML is generated: the Editor creates the new prefabs from this map.
"""
from pathlib import Path
import re,json
ROOT=Path(__file__).resolve().parents[2]
BASE=ROOT/'SourceAssets/BiologyLegacy/Assets/_Project/Prefabs/Biologia/Celulas'
def vec(body,key,default):
    m=re.search(r'^  '+key+r': \{([^}]+)\}',body,re.M)
    return {k:float(v) for k,v in re.findall(r'(\w+): ([^,]+)',m[1])} if m else default
def parse(path):
    raw=path.read_text(encoding='utf-8');objects={};transforms={};meshes={};materials={}
    for match in re.finditer(r'^--- !u!(\d+) &(-?\d+)[^\n]*\n(.*?)(?=^--- !u!|\Z)',raw,re.M|re.S):
        typ,id,body=int(match[1]),match[2],match[3]
        if typ==1:
            m=re.search(r'^  m_Name: (.*)',body,re.M);objects[id]=m[1].strip().strip('"') if m else ''
        if typ==4:
            go=re.search(r'm_GameObject: \{fileID: (-?\d+)',body)
            father=re.search(r'm_Father: \{fileID: (-?\d+)',body)
            if go and father:transforms[id]={'id':id,'go':go[1],'parent':father[1], 'position':vec(body,'m_LocalPosition',{'x':0,'y':0,'z':0}),'rotation':vec(body,'m_LocalRotation',{'x':0,'y':0,'z':0,'w':1}),'scale':vec(body,'m_LocalScale',{'x':1,'y':1,'z':1})}
        if typ in (23,33):
            go=re.search(r'm_GameObject: \{fileID: (-?\d+)',body)
            if not go:continue
            if typ==33:
                m=re.search(r'm_Mesh: \{fileID: (-?\d+), guid: ([a-f0-9]+)',body)
                if m:meshes[go[1]]={'fileID':int(m[1]),'guid':m[2]}
            else:
                section=re.search(r'm_Materials:\n(.*?)(?=^  \w)',body,re.S|re.M)
                materials[go[1]]=[{'fileID':int(a),'guid':b} for a,b in re.findall(r'fileID: (-?\d+), guid: ([a-f0-9]+)',section[1])] if section else []
    nodes=[]
    for id,t in transforms.items():
        if t['go'] not in objects:continue
        nodes.append({**t,'name':objects[t['go']],'mesh':meshes.get(t['go']), 'materials':materials.get(t['go'],[])})
    return {'name':path.stem,'nodes':nodes}
def main():
    out=ROOT/'Assets/_Project/Art/Biology/Cells/GeometryMap.json'
    cells=[parse(BASE/(name+'.prefab')) for name in ['Animal_Cell','Plant_Cell']]
    out.write_text(json.dumps({'cells':cells},ensure_ascii=False,indent=2),encoding='utf-8')
    print([(c['name'],len(c['nodes']),sum(n['mesh'] is not None for n in c['nodes'])) for c in cells])
if __name__=='__main__':main()
