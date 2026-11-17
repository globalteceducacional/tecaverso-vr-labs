"""Read-only audit of the legacy Biology corpus; reports do not require Unity."""
import json, re, hashlib
from pathlib import Path
from collections import Counter, deque

ROOT = Path(__file__).resolve().parents[2]
OLD = ROOT / 'PROJETO ANTIGO/tecaverso-vr'
ASSETS = OLD / 'Assets'
OUT = ROOT / 'docs/biology'
GUID = re.compile(r'guid: ([0-9a-f]{32})')
def text(p):
    return p.read_text(encoding='utf-8-sig', errors='replace')
def rel(p):
    return p.relative_to(OLD).as_posix()
def refs(p):
    if p.suffix.lower() not in {'.prefab','.unity','.mat','.asset','.shader','.shadergraph','.meta','.controller'}:
        return set()
    return set(GUID.findall(text(p)))
def run():
    OUT.mkdir(parents=True,exist_ok=True)
    index={}
    for p in ASSETS.rglob('*.meta'):
        match=re.search(r'^guid: ([0-9a-f]{32})',text(p),re.M)
        if match:index[match[1]]=Path(str(p)[:-5])
    scenes=[]
    for p in sorted((ASSETS/'_Project/Scenes/Biologia').glob('*.unity')):
        raw=text(p); gs=refs(p)
        scenes.append({'scene':rel(p),'bytes':p.stat().st_size,'objects':len(re.findall(r'^GameObject:',raw,re.M)),
          'names':re.findall(r'^  m_Name: (.+)',raw,re.M),
          'sources':[rel(index[g]) for g in sorted(gs) if g in index],
          'methods':sorted(set(re.findall(r'm_MethodName: (\S+)',raw)))})
    prefabs=[]
    for p in sorted((ASSETS/'_Project/Prefabs/Biologia').rglob('*.prefab')):
        raw=text(p)
        prefabs.append({'prefab':rel(p),'bytes':p.stat().st_size,
          'names':re.findall(r'^  m_Name: (.+)',raw,re.M),
          'scripts':[rel(index[g]) if g in index else 'UNRESOLVED:'+g for g in re.findall(r'm_Script: \{fileID: \d+, guid: ([0-9a-f]{32})',raw)],
          'dependencies':[rel(index[g]) for g in sorted(refs(p)) if g in index]})
    base=ASSETS/'_Project/Models/Biologia'
    art=[p for p in base.rglob('*') if p.is_file() and p.suffix!='.meta']
    # Include references from import remaps as well as materials.
    roots=list((ASSETS/'_Project/Prefabs/Biologia').rglob('*.prefab'))
    closure=set(art);queue=deque(art+roots)
    unresolved=set()
    while queue:
        p=queue.popleft();gs=refs(p)
        meta=Path(str(p)+'.meta')
        if meta.exists():gs|=refs(meta)
        for g in gs:
            if g.startswith('000000'):continue
            dep=index.get(g)
            if not dep:unresolved.add(g);continue
            if dep.is_file() and dep.suffix.lower() not in {'.cs','.dll','.unity','.prefab'} and dep not in closure:
                closure.add(dep);queue.append(dep)
    packages={}
    for p in (ROOT/'Library/PackageCache').glob('*'):
        if not p.name.startswith(('com.unity.render-pipelines.universal@','com.unity.xr.interaction.toolkit@')):continue
        for meta in p.rglob('*.meta'):
            m=re.search(r'^guid: ([0-9a-f]{32})',text(meta),re.M)
            if m:packages[m[1]]=meta.relative_to(ROOT/'Library/PackageCache').as_posix()
    data={'source_root':'PROJETO ANTIGO/tecaverso-vr','version':text(OLD/'ProjectSettings/ProjectVersion.txt').strip(),
      'nested_version':text(OLD/'tecaverso-vr/ProjectSettings/ProjectVersion.txt').strip(),
      'scenes':scenes,'prefabs':prefabs,'art':[{'path':rel(p),'bytes':p.stat().st_size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()} for p in sorted(closure)],
      'package_references':{g:packages[g] for g in sorted(unresolved) if g in packages},
      'unresolved_art_guids':sorted(unresolved-set(packages))}
    (OUT/'legacy-audit.json').write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf-8')
    print(json.dumps({'scenes':len(scenes),'prefabs':len(prefabs),'art_files':len(closure),'art_MB':round(sum(p.stat().st_size for p in closure)/1e6,2),'extensions':dict(Counter(p.suffix.lower() for p in closure)),'unresolved':data['unresolved_art_guids']},indent=2))
    for s in scenes:
        bio=[p for p in s['sources'] if '/Biologia/' in p]
        print(Path(s['scene']).stem, 'objects',s['objects'],'biology_sources',bio,'methods',s['methods'])
if __name__=='__main__':run()
