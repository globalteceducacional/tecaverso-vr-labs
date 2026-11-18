const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const { pathToFileURL } = require("node:url");
const {
  openBrowser,
} = require("../tecaverso-demo/node_modules/@remotion/renderer");

const root = path.resolve(__dirname, "../..");
const output = path.join(root, "GeneratedMedia/GDD/biologia");
const entry = pathToFileURL(
  path.join(root, "docs/prototypes/biologia/anatomy-layout.html"),
).href;

async function capture() {
  fs.mkdirSync(output, { recursive: true });
  const browser = await openBrowser("chrome", {
    browserExecutable: path.join(
      root,
      "Tools/tecaverso-demo/node_modules/.remotion/chrome-headless-shell/win64/chrome-headless-shell-win64/chrome-headless-shell.exe",
    ),
    logLevel: "error",
  });
  try {
    const page = await browser.newPage({
      context: () => null,
      logLevel: "error",
      indent: false,
      pageIndex: 0,
      onBrowserLog: null,
      onLog: () => {},
    });
    await page.setViewport({ width: 1400, height: 950, deviceScaleFactor: 1 });
    for (const state of [
      "entry",
      "detail",
      "grab",
      "path",
      "paused",
      "static",
    ]) {
      await page.goto({ url: entry + "?state=" + state, timeout: 15000 });
      await page.evaluate(async () => {
        await document.fonts.ready;
        await new Promise(requestAnimationFrame);
      });
      const view = await page.evaluate(() => ({
        controls: document.querySelectorAll(".systems button").length,
        details: !document.querySelector("#query").hidden,
        action: document.querySelector("#process").textContent,
        process: !document.querySelector("#process").hidden,
      }));
      assert.equal(view.controls, 9);
      assert.equal(
        view.details,
        ["detail", "grab", "static"].includes(state),
        state,
      );
      assert.equal(view.process, state !== "static", state);
      if (state === "paused") assert.equal(view.action, "Retomar percurso");
      const { value: shot } = await page
        ._client()
        .send("Page.captureScreenshot", {
          format: "png",
          captureBeyondViewport: true,
        });
      fs.writeFileSync(
        path.join(output, "anatomia-layout-" + state + ".png"),
        Buffer.from(shot.data, "base64"),
      );
    }

    // Real mouse events exercise click, drag, release, change of system and dial.
    await page.goto({ url: entry, timeout: 15000 });
    const mouse = (type, x, y, down = false) =>
      page._client().send("Input.dispatchMouseEvent", {
        type,
        x,
        y,
        button: type === "mouseMoved" ? "none" : "left",
        buttons: down ? 1 : 0,
        clickCount: type === "mouseMoved" ? 0 : 1,
      });
    const lung = await page.evaluate(() => {
      const rect = document.querySelector("canvas").getBoundingClientRect();
      return {
        x: rect.left + (rect.width * 267) / 480,
        y: rect.top + (rect.height * 181) / 620,
      };
    });
    await mouse("mousePressed", lung.x, lung.y, true);
    await mouse("mouseReleased", lung.x, lung.y);
    assert.equal(
      await page.evaluate(() => document.querySelector("#organ").textContent),
      "Pulmão esquerdo",
    );
    assert.equal(
      await page.evaluate(() => document.querySelector("#query").hidden),
      false,
    );
    await mouse("mousePressed", lung.x, lung.y, true);
    await mouse("mouseMoved", lung.x + 60, lung.y + 45, true);
    assert.match(
      await page.evaluate(() => document.querySelector("#status").textContent),
      /Solte para devolver/,
    );
    await mouse("mouseReleased", lung.x + 60, lung.y + 45);
    assert.equal(await page.evaluate(() => held), false);
    await page.evaluate(() =>
      document.querySelectorAll(".systems button")[3].click(),
    );
    assert.equal(
      await page.evaluate(() => document.querySelector("#query").hidden),
      true,
    );
    await page.evaluate(() => document.querySelector("#process").click());
    assert.equal(
      await page.evaluate(() => document.querySelector("#process").textContent),
      "Pausar percurso",
    );
    await page.evaluate(() => document.querySelector("#process").click());
    const paused = await page.evaluate(() => phase);
    await page.evaluate(
      () =>
        new Promise((resolve) =>
          requestAnimationFrame(() => requestAnimationFrame(resolve)),
        ),
    );
    assert.equal(await page.evaluate(() => phase), paused);
    await page.evaluate(() => document.querySelector("#process").click());
    await page.evaluate(
      () =>
        new Promise((resolve) =>
          requestAnimationFrame(() => requestAnimationFrame(resolve)),
        ),
    );
    assert.notEqual(await page.evaluate(() => phase), paused);
    await page.evaluate(() =>
      document
        .querySelector("#dial")
        .dispatchEvent(new KeyboardEvent("keydown", { key: "ArrowRight" })),
    );
    assert.notEqual(await page.evaluate(() => yaw), 0);
    await page.setViewport({ width: 390, height: 844, deviceScaleFactor: 1 });
    assert.equal(
      await page.evaluate(
        () => document.documentElement.scrollWidth <= innerWidth,
      ),
      true,
    );
    console.log(
      "Seis estados capturados. Consulta, pega e soltura, troca, pausa, retomada, giro e largura de 390 px verificados no HTML.",
    );
  } finally {
    await browser.close({ silent: true });
  }
}
capture().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
