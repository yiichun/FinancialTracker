## 🚀 系統功能與操作指南

### 第一章：安全登入系統
* **開啟軟體**：執行應用程式後，系統將顯示安全登入畫面。
* **輸入認證**：請在輸入框中輸入您的名字、暱稱或學號（例如：`s1131520`）。

<img width="350" height="210" alt="image" src="https://github.com/user-attachments/assets/ea4c5a02-ddaa-406f-b679-7bdd89db5e5c" />

* **帳號狀態判定**：
    * **舊用戶（非首次登入）**：系統若於本機偵測到歷史檔案 `records_您的名稱.json`，將會**直接放行並進入主畫面**。
    * **新用戶（首次登入）**：若本機找不到對應檔案，系統會跳出確認視窗詢問 *「是否要建立新帳號？」*：
        * 點選 **【是(Yes)】** 即可為新用戶初始化個人帳本。
        * 若因不小心打錯字導致跳出此視窗，請點選 **【否(No)】** 重新輸入正確名稱。

<img width="546" height="242" alt="image" src="https://github.com/user-attachments/assets/69e6cbc8-d155-436c-b515-5a8c9c2a764c" />

<img width="280" height="171" alt="image" src="https://github.com/user-attachments/assets/3c106bca-4e89-4230-8112-13d7ac9aab02" />

---

### 第二章：核心日常記帳
登入後即可進入系統主頁總覽，系統會即時動態統計「本月總收入」、「本月總支出」與「剩餘資產」，並即時於右上角提示當月單筆最高與最低支出。

<img width="865" height="478" alt="image" src="https://github.com/user-attachments/assets/e50cace6-6e95-4e46-9801-88d71a32148a" />

#### 1. 新增帳目明細
若您要記帳的幣值是新台幣，請直接由第二步開始；若使用外幣，則需先進行第一步。
1.  **切換幣值**：在點擊新增前，點選外幣下拉選單將 `TWD (新台幣)` 切換為其他幣值（例如：`JPY 日幣`、`USD 美金`、`KRW 韓元`、`EUR 歐元`）。

<img width="865" height="478" alt="image" src="https://github.com/user-attachments/assets/ed226118-77e5-45a2-a5d8-0a3cc534a703" />
   
2.  **開啟表單**：點擊主畫面左上角的 **`➕ 新增帳目`** 按鈕。

   <img width="613" height="383" alt="image" src="https://github.com/user-attachments/assets/2433238b-86b3-44f0-8171-9310726b5120" />

3.  **填寫資訊**：
    * **日期**：選擇交易發生的實際日期。
    * **類型**：下拉選擇 `支出` 或 `收入`。
    * **分類**：選擇內建分類。若需自訂，請點擊下拉選單最下方的 **`[➕ 新增自訂類別...]`**，並透過彈出視窗輸入新類別名稱（系統會自動保存至 `CustomCategories.txt` 中）。
    * **金額**：輸入正整數金額。
    * **備忘**：填寫交易備註（例如：排骨便當）。
4.  **確認寫入**：點擊 **【確認】** 按鈕（或按 `Enter` 鍵），系統會依據當前匯率自動轉換為台幣並寫入主畫面資料表格中。

<img width="243" height="280" alt="image" src="https://github.com/user-attachments/assets/3530ce04-5e9e-4802-b1ba-5dedfbae0406" />
<img width="371" height="211" alt="image" src="https://github.com/user-attachments/assets/fb44d1bd-7655-43e5-a2ae-c4a412df280b" />
<img width="223" height="251" alt="image" src="https://github.com/user-attachments/assets/9652e1fd-d21f-4f7f-b78e-eed58f9c0a2c" />


#### 2. 修改與刪除帳目
* **修改項目**：在下方明細表格中，對欲修改的行數**連續點擊滑鼠左鍵兩下**，或點擊上方的 **`✏️修改項目`** 按鈕，修正完畢後點擊儲存即可。

  <img width="865" height="235" alt="image" src="https://github.com/user-attachments/assets/68e8259e-0d27-47e2-824a-db18b901e45c" />

* **單筆刪除**：選中該筆明細後，點擊 **`刪除選取`**，並在系統彈出防呆確認視窗時點擊「是」。
* **多筆大量刪除**：
    * 按住 `Ctrl` 鍵不放，可點選不連續的多筆資料。
    * 按住 `Shift` 鍵不放，可批次點選區間內的所有連續資料。
    * 選好後按下 **`刪除選取`** 即可一次清空。
    * *註：若刪除的明細屬於固定記帳連動群組，則同一群組內的關聯資料也會一併被安全連帶刪除。*

<img width="585" height="180" alt="image" src="https://github.com/user-attachments/assets/dec0b81c-5b4b-4d3f-8805-d74fb1b61988" />
<img width="807" height="371" alt="image" src="https://github.com/user-attachments/assets/29f60898-93ac-4968-9783-2da61348facd" />


#### 3. 系統核心進階工具
* **⚠️ 清除所有資料**：一鍵清空目前登入帳號內的所有記帳紀錄（具備防誤觸二次確認警告）。
* **📊 匯出資料**：點擊按鈕後可自由選擇存放路徑，系統會將所有帳目結構化整理並匯出為標準的 `.csv` 試算表檔案。

  <img width="260" height="198" alt="image" src="https://github.com/user-attachments/assets/bc56f7fb-3b7e-4fdc-afd9-40d11e0fae1a" />
  <img width="314" height="149" alt="image" src="https://github.com/user-attachments/assets/e91d4f1c-3a46-4a9f-9b01-68be5bb6ad9a" />
  <img width="448" height="209" alt="image" src="https://github.com/user-attachments/assets/2412c244-cc31-4cba-9017-6c9cd9ee9b20" />

* **📂 備份還原**：可自由載入舊有的歷史備份檔案將資產資料救回（若已執行「清除所有資料」且無外部備份則無法復原）。
* **💾 儲存檔案**：系統具備關閉時自動儲存機制，使用者亦可隨時手動點擊儲存確保資料完好。
* **📅 固定記帳功能**：針對如 Netflix 訂閱費、手機電信費或每月固定薪資，免去手動重複輸入：
    1.  填寫收支類型、備忘、金額與分類，並設定「自動執行週期」（如：30天）與首次執行時間。
    2.  按下 **【確認】** 後，系統將在安全範圍內自動預算多筆排程明細，並動態推算與寫入未來的帳目明細中。

<img width="245" height="277" alt="image" src="https://github.com/user-attachments/assets/d1dcdb64-b6bf-46ea-9ced-d2fbf0726772" />
<img width="212" height="130" alt="image" src="https://github.com/user-attachments/assets/c5deebfe-7480-4fa8-abbc-88aa8760c4d4" />
<img width="569" height="190" alt="image" src="https://github.com/user-attachments/assets/85e93dcf-a5c9-4f48-8b8e-c81d9afc85f6" />


#### 4. 歷史紀錄高級檢索與排序
* **條件篩選**：可複合透過「分類選單」、「🔍 輸入關鍵字搜尋Memo」TextBox 以及勾選「篩選日期」日曆區間進行精準過濾。

<img width="579" height="158" alt="image" src="https://github.com/user-attachments/assets/31d14fcf-5ddd-470d-b853-0fa8f11a20ef" />
<img width="213" height="175" alt="image" src="https://github.com/user-attachments/assets/a3737ccd-d44f-46d7-ab36-6c55b34bc9f3" />
<img width="188" height="160" alt="image" src="https://github.com/user-attachments/assets/ee6323e2-c5e7-48cf-9f34-58625c6e1dd8" />

* **極速排序**：
    * 點擊表格欄位 **`Date`**：可於「近到遠」或「遠到近」的時間順序中切換。
    * 點擊表格欄位 **`Amount`**：可於「金額大到小」或「金額小到大」的權重順序中切換。

---

### 第三章：精準預算控制系統
* **設定限額**：切換至 **【預算設定】** 頁籤。
* **客製化預算**：系統初始預設每月預算為 `10,000 元`，使用者可依自身經濟需求自由調升或調降並點擊更新。
* **動態進度條**：主介面會同步呈現動態 `ProgressBar` 預算進度條，直覺反射剩餘預算百分比。若當月開銷不慎超出預算上限，系統會觸發警示對話框進行防度超支控管。

<img width="853" height="533" alt="image" src="https://github.com/user-attachments/assets/d14f2a4d-b17b-4b12-9c96-c49a3e85a8a5" />
<img width="538" height="253" alt="image" src="https://github.com/user-attachments/assets/2423aade-491b-43e9-b987-bdd64a91dc0d" />
<img width="445" height="164" alt="image" src="https://github.com/user-attachments/assets/a25d91d4-731b-4e32-a50a-08c0e4b4500b" />


---

### 第四章：多元視覺化統計圖表
切換至 **【統計圖表】** 頁籤，左上角選單支援三種維度的統計圖切換與時間軸過濾：
1.  **分類支出比例 (圓餅圖)**：完美呈現花費結構權重，一眼抓出哪類消費占用了最高的資金。
   
<img width="238" height="119" alt="image" src="https://github.com/user-attachments/assets/82f0bc6f-0801-4d5b-bd67-885085330e10" />

2.  **當月收支總覽 (長條圖)**：水平橫向直觀對比當月「總收入」與「總支出」的淨值差額。

   <img width="255" height="136" alt="image" src="https://github.com/user-attachments/assets/2514ef44-d154-4e8f-8873-0860f9bbb8a6" />

3.  **歷史收支趨勢 (折線圖)**：支援自訂「開始年月」與「結束年月」區間，動態繪製連續月份的資產起伏與消費趨勢線。

<img width="661" height="470" alt="image" src="https://github.com/user-attachments/assets/0e43c6bc-b691-4d50-9dea-3326908b19c2" />

---

### 第五章：智慧財務診斷系統
* **財務健康評分**：切換至 **【財務診斷】** (財務分析) 頁籤。
* **大數據評估**：系統內建演算法會針對使用者的 `預算控制率`、`每月儲蓄率` 及 `大額單筆怪獸消費` 進行即時深度審查，並精算產出一個消逝評分（0~100 分）。
* **專屬理財建議**：
    * **👑 財務狀態：卓越 (Excellent)** (85分以上) -> 顯示綠色指引，表彰流暢的金流掌控。
    * **⚖️ 財務狀態：穩定 (Stable)** (70~84分) -> 顯示黃褐色提示，提醒部分開銷可適度優化。
    * **🚨 財務狀態：危險 (High Risk)** (70分以下) -> 觸發紅色警告，警示大額衝動消費或預算超支，並提供強迫實施「24小時冷靜期」等具體理財建議。

<img width="759" height="474" alt="image" src="https://github.com/user-attachments/assets/3716a8e4-b13d-4be2-9fd5-c21f5654d5c3" />

---

### 第六章：自訂外幣匯率配置
* **即時匯率表**：切換至 **【匯率設定】** 頁籤，查看系統內建包含 `USD`, `JPY`, `KRW`, `EUR` 等基準外匯資料。
* **動態覆蓋修改**：直接在表格中對特定貨幣的數字**連續點擊兩下**即可手動修改最新匯率。
* **持久化儲存**：修改完成後，必須點擊右下角的 **`💾 儲存匯率配置`** 按鈕。系統將跳出成功配置視窗並將新參數持久化保存，後續所有多幣值記帳換算均會以此新匯率為基準。

<img width="527" height="378" alt="image" src="https://github.com/user-attachments/assets/9dcd5cfb-6fea-48ee-9767-f3bd03900ae9" />
