# NetCoreSampleProject
Development Tool: Visual Studio 2019<br>
.Net Core Version: 3.1<br>
範例方案包含以下專案<br>
* Domain
* DataLayer
* BusinessLayer
* Backend
* Batch
## Domain 核心服務層
專案相依：無 <br>
分層結構 <br>
* AbstractInterceptor
> 說明：AOP攔截器 <br>
> 命名規則：名稱加上後綴***AopAttribute***，如：\[Name\]***AopAttribute*** <br>
* Config
> 說明：設定檔結構 <br>
> 命名規則：名稱加上後綴***Config***，如：\[Name\]***Config*** <br>
* DatabaseContext
> 說明：資料庫物件 <br>
> 命名規則：名稱加上後綴***DbContext***，如：\[Name\]***DbContext*** <br>
* Entity
> 說明：資料表物件 <br>
> 命名規則：依照資料表名稱命名，如：\[Name\] <br>
* Enum
> 說明：列舉 <br>
> 命名規則：名稱加上後綴***Enum***，如：\[Name\]***Enum*** <br>
* IService
> 說明：核心服務介面 <br>
> 命名規則：名稱加上前綴***I***，名稱加上後綴 ***Service***，如：***I***\[Name\]***Service*** <br>
* Model
> 說明：公用資料結構 <br>
> 命名規則：* 名稱加上前綴***Common***，輸入名稱加上後綴***Model***，如：***Common***\[Name\]***Model*** <br>
* Service
> 說明：核心服務實作 <br>
> 命名規則：名稱加上後綴***Service***，如：\[Name\]***Service*** <br>
* Util
> 說明： <br>
> 名稱加上後綴***Util***，命名規則：\[Name\]，如：\[Name\]***Util*** <br>
## DataLayer 資料處理層
專案相依：Domain <br>
負責存取資料庫或快取服務 <br>
分層結構 <br>
* IManager
> 說明：資料存取介面 <br>
> 命名規則：名稱加上前綴***I***，名稱加上後綴 ***Manager***，如：\[Name\]***Manager*** <br>
* Manager
> 說明：資料存取實作 <br>
> 命名規則：名稱加上後綴***Manager***，如：\[Name\]***Manager*** <br>
* Model
> 說明：資料結構 <br>
> 命名規則：名稱加上中綴***Manager***，輸入名稱加上後綴***Model***或***InputModel***，輸出名稱加上後綴***Dto***或***OutputModel***，如：\[Name\]***Manager***\[Method\]***Model***、\[Name\]***Manager***\[Method\]***Dto***、\[Name\]***Manager***\[Method\]***InputModel***、\[Name\]***Manager***\[Method\]***OutputModel*** <br>
## BusinessLayer 邏輯處理層
專案相依：DataLayer、BusinessLayer <br>
負責處理業務邏輯 <br>
分層結構 <br>
* ILogic
> 說明：資料存取介面 <br>
> 命名規則：名稱加上前綴***I***，名稱加上後綴 ***Logic***，如：\[Name\]***Logic*** <br>
* Logic
> 說明：資料存取實作 <br>
> 命名規則：名稱加上後綴***Logic***，如：\[Name\]***Logic*** <br>
* Model
> 說明：資料結構 <br>
> 命名規則：名稱加上中綴***Logic***，輸入名稱加上後綴***InputModel***，輸出名稱加上後綴***OutputModel***，如：\[Name\]***Logic***\[Method\]***InputModel***、\[Name\]***Logic***\[Method\]***OutputModel***  <br>
* Mapper
> 說明：資料結構轉換 <br>
> 命名規則：名稱加上後綴***MapperConfiguration***，如：\[Name\]***MapperConfiguration*** <br>
## Backend Web服務層
專案相依：Domain、DataLayer、BusinessLayer <br>
分層結構 <br>
* Controller
> 說明：服務進入點 <br>
> 命名規則：* 名稱加上後綴***Controller***，如：\[Name\]***Controller*** <br>
* Middleware
> 說明：服務請求與回應攔截 <br>
> 命名規則：* 名稱加上後綴***Middleware***，如：\[Name\]***Middleware*** <br>
* AuthorizationFilter
> 說明：最優先執行，通常驗證Request合不合法 <br>
> 命名規則：* 名稱加上後綴***AuthorizationFilter***，如：\[Name\]***AuthorizationFilter*** <br>
* ResourceFilter
> 說明：Model Binding前執行 <br>
> 命名規則：* 名稱加上後綴***ResourceFilter***，如：\[Name\]***ResourceFilter*** <br>
* ActionFilter
> 說明：Model Binding後執行 <br>
> 命名規則：名稱加上後綴***ActionFilter***，如：\[Name\]***ActionFilter*** <br>
* ResultFilter
> 說明：Action完成後執行 <br>
> 命名規則：名稱加上後綴***ResultFilter***，如：\[Name\]***ResultFilter*** <br>
* ExceptionFilter
> 說明：發生異常時執行 <br>
> 命名規則：名稱加上後綴***ExceptionFilter***，如：\[Name\]***ExceptionFilter*** <br>
* Model
> 說明：資料結構 <br>
> 命名規則：輸入名稱加上後綴***InputModel***，輸出名稱加上後綴***OutputModel***，如：\[Name\]\[Method\]***InputModel***、\[Name\]\[Method\]***OutputModel*** <br>
## Batch 批次程式
專案相依：Domain、DataLayer、BusinessLayer <br>
分層結構 <br>
* Runner
> 服務進入點 <br>
> 命名規則：* 名稱加上後綴***Runner***，如：\[Name\]***Runner*** <br>
* Model
> 說明：資料結構 <br>
> 命名規則：輸入名稱加上後綴***InputModel***，* 輸出名稱加上後綴***OutputModel***，如：\[Name\]\[Method\]***InputModel***、 \[Name\]\[Method\]***OutputModel***  <br>
* Mapper
> 說明：資料結構轉換 <br>
> 命名規則：名稱加上後綴***MapperConfiguration***，如：\[Name\]***MapperConfiguration*** <br>