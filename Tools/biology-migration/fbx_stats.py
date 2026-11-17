"""Inspect binary FBX topology without importing or changing the files."""
import struct,zlib,json
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
def inspect(path):
    b=path.read_bytes()
    if not b.startswith(b'Kaydara FBX Binary'):return {'format':'not binary'}
    version=struct.unpack_from('<I',b,23)[0];wide=version>=7500;header=25 if wide else 13
    vertices=polygons=triangles=geometries=0;names=[]
    def node(pos):
        nonlocal vertices,polygons,triangles,geometries
        if pos+header>len(b):return len(b)
        end,count,plen,nlen=struct.unpack_from('<QQQB' if wide else '<IIIB',b,pos)
        if not end:return pos+header
        name=b[pos+header:pos+header+nlen].decode('utf-8','replace');cur=pos+header+nlen
        if name=='Geometry':geometries+=1
        for i in range(count):
            kind=chr(b[cur]);cur+=1
            if kind in 'YCLFDI':cur+={'Y':2,'C':1,'L':8,'F':4,'D':8,'I':4}[kind]
            elif kind in 'SR':
                length=struct.unpack_from('<I',b,cur)[0];cur+=4
                if name=='Model' and kind=='S':names.append(b[cur:cur+length].decode('utf-8','replace').split('\x00')[0])
                cur+=length
            elif kind in 'fdlibc':
                length,encoding,clen=struct.unpack_from('<III',b,cur);cur+=12
                if name=='Vertices':vertices+=length//3
                if name=='PolygonVertexIndex' and kind=='i':
                    raw=b[cur:cur+clen];raw=zlib.decompress(raw) if encoding else raw
                    run=0
                    for (v,) in struct.iter_unpack('<i',raw):
                        run+=1
                        if v<0:polygons+=1;triangles+=max(0,run-2);run=0
                cur+=clen
            else:raise ValueError(kind)
        while cur<end-header:cur=node(cur)
        return end
    pos=27
    while pos<len(b)-header:
        if not any(b[pos:pos+header]):break
        nxt=node(pos)
        if nxt<=pos:break
        pos=nxt
    return {'version':version,'coverage':'parsed' if geometries else 'unsupported geometry layout','geometries':geometries,'vertices':vertices,'polygons':polygons,'triangles_fan_estimate':triangles,'model_nodes':names}
if __name__=='__main__':
    base=ROOT/'SourceAssets/BiologyLegacy/Assets/_Project/Models/Biologia'
    data={p.relative_to(base).as_posix():inspect(p) for p in sorted(base.rglob('*')) if p.suffix.lower()=='.fbx'}
    (ROOT/'docs/biology/model-topology.json').write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf-8')
    for k,v in data.items():print(Path(k).name,v.get('geometries','unparsed'),v.get('triangles_fan_estimate','unparsed'))
