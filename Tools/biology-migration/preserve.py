"""Preserve audited Biology source assets without importing the legacy project into Unity.

Run after audit.py. Never overwrites a different existing file. Does not delete sources.
"""
from pathlib import Path
import json, hashlib, shutil
ROOT=Path(__file__).resolve().parents[2]
OLD=ROOT/'PROJETO ANTIGO/tecaverso-vr'
DEST=ROOT/'SourceAssets/BiologyLegacy'
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def main():
    audit=json.loads((ROOT/'docs/biology/legacy-audit.json').read_text(encoding='utf-8'))
    paths={x['path'] for x in audit['art']}
    paths.update(x['prefab'] for x in audit['prefabs'])
    paths.update(x['scene'] for x in audit['scenes'])
    paths.update(p.relative_to(OLD).as_posix() for p in (OLD/'Assets/_Project/Scripts').rglob('*.cs'))
    paths.add('ProjectSettings/ProjectVersion.txt')
    paths.add('Packages/manifest.json')
    for item in list(paths):
        if (OLD/(item+'.meta')).exists():paths.add(item+'.meta')
        parent=Path(item).parent
        while parent.as_posix() not in ('.','Assets','ProjectSettings','Packages'):
            meta=OLD/(parent.as_posix()+'.meta')
            if meta.exists():paths.add(meta.relative_to(OLD).as_posix())
            parent=parent.parent
    rows=[]
    for item in sorted(paths):
        src=OLD/item;dst=DEST/item
        if not src.is_file():raise RuntimeError('Missing source '+item)
        digest=sha(src)
        if dst.exists() and sha(dst)!=digest:raise RuntimeError('Refusing overwrite '+str(dst))
        dst.parent.mkdir(parents=True,exist_ok=True)
        if not dst.exists():shutil.copy2(src,dst)
        if sha(dst)!=digest:raise RuntimeError('Copy verification failed '+item)
        rows.append({'path':item,'bytes':src.stat().st_size,'sha256':digest})
    (DEST/'manifest.json').write_text(json.dumps({'source':audit['source_root'],'files':rows,'verified_sha256':True},ensure_ascii=False,indent=2),encoding='utf-8')
    print('Verified',len(rows),'files;',round(sum(x['bytes'] for x in rows)/1e6,2),'MB. Sources preserved; nothing deleted.')
if __name__=='__main__':main()
