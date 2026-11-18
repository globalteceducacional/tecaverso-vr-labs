const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const { pathToFileURL } = require("node:url");
const { openBrowser } = require("../tecaverso-demo/node_modules/@remotion/renderer");

const root = path.resolve(__dirname, "../..");
const url = pathToFileURL(path.join(root, "docs/prototypes/biologia/index.html")).href + "?lab=ecossistemas";
const output = path.join(root, "GeneratedMedia/GDD/biologia");

async function review() {
  const browser = await openBrowser("chrome", {
    browserExecutable: path.join(root, "Tools/tecaverso-demo/node_modules/.remotion/chrome-headless-shell/win64/chrome-headless-shell-win64/chrome-headless-shell.exe"),
    logLevel: "error",
  });
  try {
    const page = await browser.newPage({ context: () => null, logLevel: "error", indent: false, pageIndex: 0, onBrowserLog: null, onLog: () => {} });
    await page.setViewport({ width: 1440, height: 1000, deviceScaleFactor: 1 });
    await page.goto({ url, timeout: 15000 });
    const click = (selector) => page.evaluate((key) => document.querySelector(key).dispatchEvent(new MouseEvent("click",{bubbles:true})), selector);
    const flowCount = () => page.evaluate(() => document.querySelectorAll(".flowpath").length);
    await click("#controls button");
    assert.equal(await flowCount(),0);
    await click("[data-eco-piece='2']"); await click("[data-eco-slot='0']");
    assert.equal(await page.evaluate(() => s.ecoHeld),2);
    for(let i=0;i<3;i++) {
      await click("[data-eco-piece='"+i+"']"); await click("[data-eco-slot='"+i+"']");
      assert.equal(await page.evaluate(()=>connectedEco()),i+1);
      assert.ok(await flowCount()>0);
    }
    await click("[data-eco-piece='0']");
    assert.equal(await flowCount(),0);
    await click("[data-eco-slot='0']");
    assert.equal(await page.evaluate(()=>connectedEco()),3);
    await click("[data-part='nutrients']");
    assert.equal(await page.evaluate(()=>document.querySelector("#detail").hidden),false);
    fs.mkdirSync(output,{recursive:true});
    for(const state of ["entry","action","detail","error","reset"]) {
      await page.goto({url:url+"&state="+state,timeout:15000});
      await page.evaluate(async()=>{await document.fonts.ready;await new Promise(requestAnimationFrame);});
      const {value:shot}=await page._client().send("Page.captureScreenshot",{format:"png",captureBeyondViewport:true});
      fs.writeFileSync(path.join(output,"ecossistemas-"+state+".png"),Buffer.from(shot.data,"base64"));
    }
    console.log("OK · encaixe direto, papel incorreto, retirada, fluxo, consulta e cinco prévias.");
  } finally { await browser.close({silent:true}); }
}
review().catch(error=>{console.error(error);process.exitCode=1;});
