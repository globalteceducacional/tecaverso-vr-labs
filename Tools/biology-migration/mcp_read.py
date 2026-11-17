"""Read-only MCP audit client for the user-started localhost Unity server.

Session state and responses are kept in ignored tmp/, never in source control.
"""
import json,urllib.request,sys
sys.stdout.reconfigure(encoding='utf-8')
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
STATE=ROOT/'tmp/biology-mcp/session.json'
def request(method,params,session=None):
    body=json.dumps({'jsonrpc':'2.0','id':1,'method':method,'params':params}).encode()
    headers={'Content-Type':'application/json','Accept':'application/json, text/event-stream'}
    if session:headers['Mcp-Session-Id']=session
    with urllib.request.urlopen(urllib.request.Request('http://127.0.0.1:8080/mcp',body,headers),timeout=35) as r:
        raw=r.read().decode();sid=r.headers.get('Mcp-Session-Id')
    if any(x.startswith('data: ') for x in raw.splitlines()):
        messages=[json.loads(x[6:]) for x in raw.splitlines() if x.startswith('data: ')]
        payload=next(x for x in messages if x.get('id')==1)
    else:payload=json.loads(raw)
    return payload,sid
def main():
    if not STATE.exists():
        _,sid=request('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'biology-read-only-audit','version':'1'}})
        STATE.parent.mkdir(parents=True,exist_ok=True);STATE.write_text(json.dumps({'session':sid}))
    sid=json.loads(STATE.read_text())['session']
    kind=sys.argv[1]
    if kind=='resource':method,params='resources/read',{'uri':sys.argv[2]}
    elif kind=='tools':method,params='tools/list',{}
    elif kind=='list':method,params='resources/list',{}
    elif kind=='call':
        name=sys.argv[2];args=json.loads(Path(sys.argv[3]).read_text(encoding='utf-8-sig'))
        allowed={'set_active_instance':None,'read_console':{'get'},'manage_scene':{'get_active','get_hierarchy'},'manage_asset':{'get_info','search'}}
        if name not in allowed or (allowed[name] is not None and args.get('action') not in allowed[name]):raise ValueError('Read-only tool/action required')
        method,params='tools/call',{'name':name,'arguments':args}
    else:raise ValueError(kind)
    payload,_=request(method,params,sid)
    out=STATE.parent/(kind+'-'+(sys.argv[2].split('/')[-1].replace(':','_') if len(sys.argv)>2 else 'list')+'.json')
    out.write_text(json.dumps(payload,ensure_ascii=False,indent=2),encoding='utf-8')
    print(json.dumps(payload,ensure_ascii=False,indent=2))
if __name__=='__main__':main()
