// Offline video QA: decode the full recording and inspect frames across every chapter.
const fs=require('node:fs'),path=require('node:path'),{spawnSync}=require('node:child_process'),{pathToFileURL}=require('node:url');
const {openBrowser}=require('../tecaverso-demo/node_modules/@remotion/renderer');
const root=path.resolve(__dirname,'../..');
const ffmpeg=path.join(root,'Tools/tecaverso-demo/node_modules/@remotion/compositor-win32-x64-msvc/ffmpeg.exe');
const video=path.resolve(root,process.argv[2]||'Recordings/Biology/02-Corpo-humano-e-sistemas-inspecao.mp4');
const out=path.join(root,'GeneratedMedia/AnatomyVideo',path.basename(video,'.mp4'));
const reviewSeconds=[3,7,16,26,35,36,39,50,59,62,65,69,73,84,94,103,107,118,127,130,134,137,141,150,152,154,156,158,160,166,172,178,184,188,192,195,198,202];
function run(args){const r=spawnSync(ffmpeg,['-hide_banner','-loglevel','error',...args],{encoding:'utf8',windowsHide:true});if(r.status!==0)throw new Error(r.stderr||r.error||'ffmpeg '+r.status);}
(async()=>{
 fs.mkdirSync(out,{recursive:true});
 const probe=spawnSync(path.join(path.dirname(ffmpeg),'ffprobe.exe'),['-v','error','-show_entries','format=duration','-of','default=nw=1:nk=1',video],{encoding:'utf8',windowsHide:true});
 const duration=Number(probe.stdout);
 if(probe.status!==0||!(duration>0))throw new Error(probe.stderr||'Duração de vídeo indisponível');
 const times=reviewSeconds.filter(t=>t<duration);
 run(['-xerror','-err_detect','explode','-i',video,'-map','0:v:0','-c:v','rawvideo','-f','null','NUL']);
 for(const t of times)run(['-y','-ss',String(t),'-i',video,'-frames:v','1','-vf','scale=960:540','-update','1',path.join(out,`frame-${t}.png`)]);
 const browser=await openBrowser('chrome',{browserExecutable:path.join(root,'Tools/tecaverso-demo/node_modules/.remotion/chrome-headless-shell/win64/chrome-headless-shell-win64/chrome-headless-shell.exe'),logLevel:'error'});
 try{
  const page=await browser.newPage({context:()=>null,logLevel:'error',indent:false,pageIndex:0,onBrowserLog:null,onLog:()=>{}});
  await page.setViewport({width:1960,height:1250,deviceScaleFactor:1});
  for(let start=0;start<times.length;start+=16){
   const subset=times.slice(start,start+16),html=path.join(out,`qa-${start}.html`);
   fs.writeFileSync(html,'<!doctype html><meta charset="utf-8"><style>body{margin:8px;background:#091324;color:white;font:18px sans-serif}main{display:grid;grid-template-columns:repeat(4,480px);gap:8px}figure{margin:0}img{width:480px;height:270px;display:block}figcaption{padding:4px}</style><main>'+subset.map(t=>`<figure><img src="frame-${t}.png"><figcaption>${Math.floor(t/60)}:${String(t%60).padStart(2,'0')}</figcaption></figure>`).join('')+'</main>');
   await page.goto({url:pathToFileURL(html).href,timeout:15000});
   await page.evaluate(()=>Promise.all([...document.images].map(i=>i.decode())));
   const {value:r}=await page._client().send('Page.captureScreenshot',{format:'png',captureBeyondViewport:true});
   fs.writeFileSync(path.join(out,`qa-contact-${start/16+1}.png`),Buffer.from(r.data,'base64'));
  }
 }finally{await browser.close({silent:true});}
 fs.writeFileSync(path.join(out,'video-qa.json'),JSON.stringify({video,duration,decodedWithoutErrors:true,framesReviewed:times},null,2));
 console.log(JSON.stringify({decodedWithoutErrors:true,frames:times.length,contactSheets:Math.ceil(times.length/16),out}));
})().catch(e=>{console.error(e);process.exitCode=1;});
