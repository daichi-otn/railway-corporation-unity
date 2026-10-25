using UnityEngine;

namespace RailwayCorporation
{
    public sealed class RailwayCompanyGame : MonoBehaviour
    {
        private enum Page { Title, Company, Operation }
        private Page screen = Page.Title;
        private Camera mainCamera;
        private float speed;
        private int power;
        private float distance = 1.2f;
        private bool running;
        private int month = 1;
        private long cash = 120000000;
        private long assets = 56000000;
        private long debt;
        private int vehicles = 1;
        private int staff = 1;
        private string notice = "会社を設立しました。最初の運行を始める準備が整っています。";
        private GUIStyle titleStyle;
        private GUIStyle headingStyle;
        private GUIStyle moneyStyle;
        private GUIStyle bodyStyle;
        private GUIStyle buttonStyle;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            BuildStyles();
            try { BuildWorld(); }
            catch (System.Exception exception) { Debug.LogException(exception); }
        }

        private void Update()
        {
            if (!running) return;
            speed = Mathf.Clamp(speed + power * 7f * Time.deltaTime - (power == 0 ? 0.6f : 0f) * Time.deltaTime, 0f, 100f);
            distance = Mathf.Max(0f, distance - speed * Time.deltaTime / 3600f);
            if (distance <= 0f) { running = false; speed = 0f; notice = "港町駅に到着しました。運行収入を会社に反映しました。"; cash += 1800000; }
        }

        private void BuildStyles()
        {
            titleStyle = new GUIStyle { fontSize = 58, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft, normal = { textColor = new Color(.92f, .97f, 1f) } };
            headingStyle = new GUIStyle { fontSize = 26, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
            moneyStyle = new GUIStyle { fontSize = 32, fontStyle = FontStyle.Bold, normal = { textColor = new Color(.3f, .84f, .88f) } };
            bodyStyle = new GUIStyle { fontSize = 15, wordWrap = true, normal = { textColor = new Color(.75f, .83f, .88f) } };
            buttonStyle = new GUIStyle { fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white }, hover = { textColor = new Color(.3f, .84f, .88f) } };
        }

        private void OnGUI()
        {
            var w = UnityEngine.Screen.width; var h = UnityEngine.Screen.height;
            if (screen == Page.Title) DrawTitle(w, h);
            else if (screen == Page.Company) DrawCompany(w, h);
            else DrawOperation(w, h);
        }

        private void DrawTitle(int w, int h)
        {
            GUI.Box(new Rect(0, 0, w, h), "");
            GUI.Label(new Rect(w * .12f, h * .2f, w * .7f, 30), "METROPOLITAN RAILWAY CORPORATION", bodyStyle);
            GUI.Label(new Rect(w * .12f, h * .27f, w * .7f, 150), "RAILWAY\nCORPORATION", titleStyle);
            GUI.Label(new Rect(w * .12f, h * .52f, 500, 40), "走らせる会社を、育てる。", headingStyle);
            if (Button(new Rect(w * .12f, h * .63f, 260, 55), "会社を経営する  →")) screen = Page.Company;
            if (Button(new Rect(w * .12f, h * .71f, 260, 48), "運転を始める  →")) screen = Page.Operation;
        }

        private void DrawCompany(int w, int h)
        {
            GUI.Label(new Rect(35, 25, 500, 35), "メトロポリタン鉄道株式会社", headingStyle);
            if (Button(new Rect(w - 180, 24, 145, 38), "運転する →")) screen = Page.Operation;
            GUI.Label(new Rect(35, 82, 320, 35), "MONTH " + month, bodyStyle);
            GUI.Label(new Rect(35, 118, 500, 45), "会社を育てる", headingStyle);
            GUI.Box(new Rect(w - 320, 70, 285, 120), "");
            GUI.Label(new Rect(w - 295, 86, 230, 22), "現金預金", bodyStyle);
            GUI.Label(new Rect(w - 295, 112, 260, 45), Yen(cash), moneyStyle);
            GUI.Label(new Rect(35, 205, 160, 22), "月次収支", bodyStyle); GUI.Label(new Rect(35, 230, 220, 38), Yen(MonthlyProfit()), moneyStyle);
            GUI.Label(new Rect(270, 205, 160, 22), "固定資産", bodyStyle); GUI.Label(new Rect(270, 230, 220, 38), Yen(assets), moneyStyle);
            GUI.Label(new Rect(505, 205, 160, 22), "借入残高", bodyStyle); GUI.Label(new Rect(505, 230, 220, 38), Yen(debt), moneyStyle);
            GUI.Label(new Rect(740, 205, 160, 22), "車両 / 社員", bodyStyle); GUI.Label(new Rect(740, 230, 220, 38), vehicles + " / " + staff, moneyStyle);
            GUI.Box(new Rect(35, 292, w - 70, h - 330), "");
            GUI.Label(new Rect(60, 315, 500, 32), "資金繰り", headingStyle);
            GUI.Label(new Rect(60, 362, 360, 120), "運賃・定期券収入\n人件費\n車両・設備維持費\n電力・管理費\n減価償却費", bodyStyle);
            GUI.Label(new Rect(420, 362, 230, 120), Yen(2800000) + "\n-" + Yen(staff * 360000) + "\n-" + Yen(vehicles * 680000) + "\n-" + Yen(900000) + "\n-" + Yen(assets / 120), bodyStyle);
            if (Button(new Rect(60, h - 95, 150, 42), "翌月へ進む")) AdvanceMonth();
            if (Button(new Rect(225, h - 95, 180, 42), "車両を購入 ¥32M")) BuyVehicle();
            if (Button(new Rect(420, h - 95, 165, 42), "運転士を採用 ¥360K")) HireDriver();
            if (Button(new Rect(600, h - 95, 175, 42), "ローンを借りる ¥5M")) TakeLoan();
            GUI.Label(new Rect(60, h - 145, w - 120, 35), notice, bodyStyle);
        }

        private void DrawOperation(int w, int h)
        {
            if (Button(new Rect(28, 24, 150, 38), "← 会社経営")) screen = Page.Company;
            GUI.Box(new Rect(28, 80, 126, 84), ""); GUI.Label(new Rect(40, 91, 100, 20), "SPEED", bodyStyle); GUI.Label(new Rect(40, 112, 100, 35), Mathf.RoundToInt(speed).ToString(), moneyStyle);
            GUI.Box(new Rect(166, 80, 175, 84), ""); GUI.Label(new Rect(178, 91, 150, 20), "NEXT STATION", bodyStyle); GUI.Label(new Rect(178, 112, 150, 30), "港町 " + distance.ToString("0.00") + " km", bodyStyle);
            GUI.Box(new Rect(w * .5f - 340, h - 110, 680, 80), "");
            if (Button(new Rect(w * .5f - 315, h - 88, 120, 38), "非常ブレーキ")) { power = -2; speed = Mathf.Max(0, speed - 12); }
            GUI.Label(new Rect(w * .5f - 170, h - 88, 80, 20), "マスコン " + PowerName(), bodyStyle);
            power = GUI.HorizontalSlider(new Rect(w * .5f - 170, h - 58, 290, 20), power, -2, 4).RoundToInt();
            if (Button(new Rect(w * .5f + 145, h - 88, 120, 38), running ? "走行中" : "発車")) running = true;
        }

        private bool Button(Rect rect, string text)
        {
            var pressed = GUI.Button(rect, GUIContent.none);
            GUI.Label(rect, text, buttonStyle);
            return pressed;
        }
        private string PowerName() { return power < 0 ? "B" + -power : power == 0 ? "N" : "P" + power; }
        private long MonthlyProfit() { return 2800000 - staff * 360000 - vehicles * 680000 - 900000 - assets / 120 - debt * 42 / 12000; }
        private void AdvanceMonth() { var profit = MonthlyProfit(); cash += profit; assets = System.Math.Max(0L, assets - assets / 120); month++; notice = month + "月の決算を完了。" + (profit >= 0 ? "黒字 " : "赤字 ") + Yen(profit) + "です。"; }
        private void BuyVehicle() { if (cash < 32000000) { notice = "現金が不足しています。"; return; } cash -= 32000000; assets += 32000000; vehicles++; notice = "新型車両を購入しました。"; }
        private void HireDriver() { if (cash < 360000) { notice = "採用費を支払えません。"; return; } cash -= 360000; staff++; notice = "運転士を採用しました。"; }
        private void TakeLoan() { cash += 5000000; debt += 5000000; notice = "設備投資ローンを借り入れました。"; }
        private static string Yen(long amount) { return "¥" + amount.ToString("N0"); }

        private void BuildWorld()
        {
            mainCamera = Camera.main;
            if (mainCamera == null) { var c = new GameObject("Main Camera"); mainCamera = c.AddComponent<Camera>(); c.tag = "MainCamera"; }
            mainCamera.transform.position = new Vector3(0, 2.4f, 3.4f); mainCamera.transform.rotation = Quaternion.Euler(-7, 0, 0); mainCamera.backgroundColor = new Color(.04f, .11f, .18f);
            var light = new GameObject("Moon Light").AddComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.2f; light.transform.rotation = Quaternion.Euler(45, -25, 0);
            CreateCube("Ground", new Vector3(0, -.25f, -70), new Vector3(50, .3f, 180), new Color(.06f, .14f, .2f));
            for (var z = -8; z > -170; z -= 6) { CreateCube("Rail", new Vector3(-1.2f, .05f, z), new Vector3(.12f, .12f, 6), Color.gray); CreateCube("Rail", new Vector3(1.2f, .05f, z), new Vector3(.12f, .12f, 6), Color.gray); CreateCube("Sleeper", new Vector3(0, 0, z), new Vector3(3, .12f, .35f), new Color(.35f, .22f, .12f)); }
            for (var z = -15; z > -170; z -= 12) { CreateCube("Catenary Pole", new Vector3(-5, 3.5f, z), new Vector3(.3f, 7, .3f), new Color(.16f, .2f, .24f)); CreateCube("Catenary Pole", new Vector3(5, 3.5f, z), new Vector3(.3f, 7, .3f), new Color(.16f, .2f, .24f)); CreateCube("Catenary Beam", new Vector3(0, 7, z), new Vector3(10, .2f, .2f), new Color(.16f, .2f, .24f)); }
            for (var z = -20; z > -160; z -= 16) { var h = 7 + Mathf.Abs(z % 5); CreateCube("City", new Vector3(-11, h / 2, z), new Vector3(5, h, 6), new Color(.05f, .16f, .25f)); CreateCube("City", new Vector3(11, h / 2, z - 4), new Vector3(6, h * .8f, 6), new Color(.07f, .2f, .3f)); }
        }

        private static void CreateCube(string label, Vector3 position, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube); go.name = label; go.transform.position = position; go.transform.localScale = scale; var renderer = go.GetComponent<Renderer>(); var source = Resources.Load<Material>("WorldMaterial"); if (source != null) renderer.material = new Material(source); renderer.material.color = color;
        }
    }

    internal static class FloatExtensions { public static int RoundToInt(this float value) { return Mathf.RoundToInt(value); } }
}
