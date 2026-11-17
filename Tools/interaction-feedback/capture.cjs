const fs=require('node:fs'),path=require('node:path');
const {pathToFileURL}=require('node:url');
const {openBrowser}=require('../tecaverso-demo/node_modules/@remotion/renderer');
(async()=>{
 const root=path.resolve(__dirname,'../..'),out=path.join(root,'GeneratedMedia/GDD/interaction-feedback');fs.mkdirSync(out,{recursive:true});
 const browser=await openBrowser('chrome',{browserExecutable:path.join(root,'Tools/tecaverso-demo/node_modules/.remotion/chrome-headless-shell/win64/chrome-headless-shell-win64/chrome-headless-shell.exe'),logLevel:'error'});
 try{const page=await browser.newPage({context:()=>null,logLevel:'error',indent:false,pageIndex:0,onBrowserLog:null,onLog:()=>{}});
 await page.setViewport({width:1600,height:1000,deviceScaleFactor:1});
 await page.goto({url:pathToFileURL(path.join(root,'docs/prototypes/interaction-feedback/index.html')).href,timeout:15000});
 await page.evaluate(()=>document.fonts.ready);
 const {value:r}=await page._client().send('Page.captureScreenshot',{format:'png',captureBeyondViewport:true});fs.writeFileSync(path.join(out,'states.png'),Buffer.from(r.data,'base64'));
 }finally{await browser.close({silent:true});}
})().catch(e=>{console.error(e);process.exitCode=1;});
