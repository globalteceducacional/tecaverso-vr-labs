// Focused browser checks and reproducible local GDD captures. No Unity build.
const fs = require("node:fs");
const path = require("node:path");
const assert = require("node:assert/strict");
const { pathToFileURL } = require("node:url");
const {
  openBrowser,
} = require("../tecaverso-demo/node_modules/@remotion/renderer");
const root = path.resolve(__dirname, "../..");
const out = path.join(root, "GeneratedMedia/GDD/biologia");
const entry = pathToFileURL(
  path.join(root, "docs/prototypes/biologia/index.html"),
).href;
const ids = [
  "celulas",
  "anatomia",
  "microbios",
  "dna",
  "ecossistemas",
  "evolucao",
];
(async () => {
  fs.mkdirSync(out, { recursive: true });
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
    await page.setViewport({ width: 1440, height: 1100, deviceScaleFactor: 1 });
    const go = async (id, state) => {
      await page.goto({
        url:
          entry + (id ? "?lab=" + id + (state ? "&state=" + state : "") : ""),
        timeout: 15000,
      });
    };
    const click = async (text) =>
      page.evaluate((t) => {
        const b = [...document.querySelectorAll("button")].find(
          (b) => b.textContent === t && !b.closest("[hidden]"),
        );
        if (!b) throw Error("Button not found: " + t);
        b.click();
      }, text);
    const message = () =>
      page.evaluate(() => document.getElementById("message").textContent);
    const capture = async (name) => {
      await page.evaluate(() => document.fonts.ready);
      const { value: metrics } = await page
        ._client()
        .send("Page.getLayoutMetrics");
      const height = Math.ceil(
        metrics.cssContentSize?.height || metrics.contentSize.height,
      );
      const { value: r } = await page
        ._client()
        .send("Page.captureScreenshot", {
          format: "png",
          captureBeyondViewport: true,
          clip: { x: 0, y: 0, width: 1440, height, scale: 1 },
        });
      fs.writeFileSync(
        path.join(out, name + ".png"),
        Buffer.from(r.data, "base64"),
      );
    };
    let checks = [];
    await go();
    assert.equal(
      await page.evaluate(() => document.querySelectorAll(".card").length),
      6,
    );
    checks.push("Six proposal entries");
    await go("dna");
    assert.equal(await page.evaluate(() => document.querySelector('[data-base="U"]') === null), true);
    await click("Base A");
    assert.match(await message(), /recusada/);
    await click("Base T");
    await click("Base A");
    await click("Base C");
    await click("Base G");
    assert.match(await message(), /concluída/);
    await click("RNA");
    assert.equal(await page.evaluate(() => document.querySelector('[data-base="T"]') === null), true);
    await click("Base C");
    assert.match(await message(), /recusada/);
    await click("Base U");
    await click("Base A");
    await click("Base C");
    await click("Base G");
    assert.match(await message(), /concluída/);
    await click("DNA");
    assert.equal(
      await page.evaluate(
        () => document.querySelectorAll("[data-slot] text")[0].textContent,
      ),
      "T",
    );
    checks.push("DNA/RNA pairing, invalid rejection, independent persistence");
    await click("Restaurar fitas");
    await click("Cancelar");
    assert.equal(
      await page.evaluate(
        () => document.querySelectorAll("[data-slot] text")[0].textContent,
      ),
      "T",
    );
    checks.push("Cancel reset preserves assembly");
    await go("ecossistemas");
    await click("Encaixar Sapo");
    assert.match(await message(), /produtor/);
    await click("Encaixar Capim");
    await click("Encaixar Gafanhoto");
    await click("Encaixar Sapo");
    await click("Exibir fluxo");
    assert.equal(
      await page.evaluate(
        () => document.querySelectorAll(".flowpath").length > 0,
      ),
      true,
    );
    checks.push("Ecology rejects wrong role and enables directed flow");
    await go("evolucao");
    await click("Guardar referência");
    await click("Ambiente claro");
    assert.equal(
      await page.evaluate(() =>
        document.getElementById("scene").textContent.includes("Claros 12"),
      ),
      true,
    );
    for (let i = 0; i < 7; i++) await click("Avançar geração");
    assert.match(await message(), /Limite/);
    assert.equal(
      await page.evaluate(() =>
        document
          .getElementById("scene")
          .textContent.includes("Referência G0: claros 12"),
      ),
      true,
    );
    checks.push(
      "Environment preserves phenotype; generation bound and immutable reference",
    );
    await go("celulas");
    await page.evaluate(() =>
      document
        .querySelector('[data-part="mito"]')
        .dispatchEvent(new MouseEvent("click", { bubbles: true })),
    );
    await click("Simular pega e afastamento da organela");
    assert.match(await message(), /afastada/);
      const detachedPose = await page.evaluate(() =>
        document.querySelector('[data-part="mito"] .part').getAttribute('transform'),
      );
      await click("Fechar consulta");
      assert.equal(await page.evaluate(() =>
        document.querySelector('[data-part="mito"] .part').getAttribute('transform'),
      ), detachedPose);
      checks.push("Closing a cell consultation preserves the detached piece");
    await click("Simular reencaixe");
    assert.match(await message(), /reencaixada/);
    checks.push("Cell selection, separation and return");
    await go("anatomia");
    await click("Observar percurso");
    assert.equal(
      await page.evaluate(
        () => document.querySelectorAll(".flowpath").length > 0,
      ),
      true,
    );
    await click("Esquelético");
    assert.equal(
      await page.evaluate(() => document.querySelectorAll(".flowpath").length),
      0,
    );
    assert.equal(
      await page.evaluate(() => document.getElementById("detail").hidden),
      true,
    );
    checks.push(
      "Anatomy switch stops irrelevant path and closes hidden selection",
    );
    await go("microbios");
    for (let i = 0; i < 2; i++) await click("Próxima etapa do fago");
    assert.equal(
      await page.evaluate(() =>
        document
          .getElementById("scene")
          .textContent.includes("Lise e liberação"),
      ),
      true,
    );
    await click("Próxima etapa do fago");
    assert.ok((await page.evaluate(() => document.getElementById("scene").textContent)).includes("Contato e entrada"));
    checks.push("Three phage stages and return to the initial stage");
    await go();
    await capture("hub");
    for (const id of ids) {
      for (const state of [
        "entry",
        "action",
        "detail",
        "error",
        ...(id === "anatomia" ? [] : ["reset"]),
      ]) {
        await go(id, state);
        await capture(id + "-" + state);
      }
    }
    await page.goto({
      url: pathToFileURL(
        path.join(root, "docs/prototypes/biologia/layout.html"),
      ).href,
      timeout: 15000,
    });
    await capture("layout");
    for (const width of [375, 768]) {
      await page.setViewport({ width, height: 1100, deviceScaleFactor: 1 });
      for (const id of ids) {
        await go(id, "detail");
        assert.equal(
          await page.evaluate(
            () => document.documentElement.scrollWidth <= innerWidth + 1,
          ),
          true,
          "Overflow " + id + " at " + width,
        );
      }
    }
    checks.push("375/768px consultation layout has no horizontal overflow");
    fs.writeFileSync(
      path.join(out, "verification.json"),
      JSON.stringify(
        {
          date: "2026-11-17",
          checks,
          scope: "HTML browser only; not Unity/XR validation",
        },
        null,
        2,
      ),
    );
    console.log(
      JSON.stringify(
        {
          passed: checks.length,
          checks,
          captures: fs.readdirSync(out).filter((x) => x.endsWith(".png"))
            .length,
        },
        null,
        2,
      ),
    );
  } finally {
    await browser.close({ silent: true });
  }
})().catch((e) => {
  console.error(e);
  process.exitCode = 1;
});
