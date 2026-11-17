"""Task-scoped MCP calls, pinned to the current Tecaverso project.

Arguments: tool-name request.json. Responses go to ignored tmp/cells/.
"""
import json,sys
from pathlib import Path
from mcp_read import request,STATE,ROOT
sys.stdout.reconfigure(encoding='utf-8')
def unpack(r):
    if 'error' in r:raise RuntimeError(r['error'])
    result=r['result']
    if 'structuredContent' in result:return result['structuredContent']
    blocks=result.get('contents',result.get('content',[]))
    for b in blocks:
        if b.get('text'):
            try:return json.loads(b['text'])
            except ValueError:return {'text':b['text']}
    return result
def main():
    sid=json.loads(STATE.read_text())['session']
    instances=unpack(request('resources/read',{'uri':'mcpforunity://instances'},sid)[0])['instances']
    targets=[i for i in instances if i['name']=='tecaverso-vr-labs']
    if len(targets)!=1:raise RuntimeError('Current project is not uniquely connected')
    request('tools/call',{'name':'set_active_instance','arguments':{'instance':targets[0]['id']}},sid)
    project=unpack(request('resources/read',{'uri':'mcpforunity://project/info'},sid)[0])['data']
    if Path(project['projectRoot']).resolve()!=ROOT.resolve():raise RuntimeError('Project path mismatch')
    name=sys.argv[1]
    params=json.loads(Path(sys.argv[2]).read_text(encoding='utf-8-sig')) if len(sys.argv)>2 else {}
    if name=='resource':r=request('resources/read',params,sid)[0]
    elif name=='tools':r=request('tools/list',{},sid)[0]
    else:r=request('tools/call',{'name':name,'arguments':params},sid)[0]
    out=ROOT/'tmp/cells';out.mkdir(parents=True,exist_ok=True)
    (out/(name+'.json')).write_text(json.dumps(r,ensure_ascii=False,indent=2),encoding='utf-8')
    print(json.dumps(unpack(r),ensure_ascii=False,indent=2))
if __name__=='__main__':main()
