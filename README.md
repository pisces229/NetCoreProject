# NetCoreProject
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
> 命名規則：[Name]***AopAttribute*** <br>
* Config
> 說明：設定檔結構 <br>
> 命名規則：[Name]***Config*** <br>
* DatabaseContext
> 說明：資料庫物件 <br>
> 命名規則：[Name]***DbContext*** <br>
* Entity
> 說明：資料表物件 <br>
> 命名規則：[Name] <br>
* Enum
> 說明：列舉 <br>
> 命名規則：[Name]***Enum*** <br>
* IService
> 說明：核心服務介面 <br>
> 命名規則：***I***[Name]***Service*** <br>
* Model
> 說明：公用資料結構 <br>
> 命名規則：***Common***[Name]***Model*** <br>
* Service
> 說明：核心服務實作 <br>
> 命名規則：[Name]***Service*** <br>
* Util
> 說明： <br>
> 命名規則：[Name]***Util*** <br>
## DataLayer 資料處理層
專案相依：Domain <br>
負責存取資料庫或快取服務 <br>
分層結構 <br>
* IManager
> 說明：資料存取介面 <br>
> 命名規則：***I***[Name]***Manager*** <br>
* Manager
> 說明：資料存取實作 <br>
> 命名規則：[Name]***Manager*** <br>
* Model
> 說明：資料結構 <br>
> 命名規則：[Name]***Manager***[Method][Input/Output]***Model*** <br>
## BusinessLayer 邏輯處理層
專案相依：DataLayer、BusinessLayer <br>
負責處理業務邏輯 <br>
分層結構 <br>
* ILogic
> 說明：資料存取介面 <br>
> 命名規則：***I***[Name]***Logic***  <br>
* Logic
> 說明：資料存取實作 <br>
> 命名規則：[Name]***Logic*** <br>
* Model
> 說明：資料結構 <br>
> 命名規則：[Name]***Logic***[Method][Input/Output]***Model*** <br>
* Mapper
> 說明：資料結構轉換 <br>
> 命名規則：[Name]***MapperConfiguration*** <br>
## Backend Web服務層
專案相依：Domain、DataLayer、BusinessLayer <br>
分層結構 <br>
* Controller
> 說明：服務進入點 <br>
> 命名規則：[Name]***Controller*** <br>
* Middleware
> 說明：服務請求與回應攔截 <br>
> 命名規則：[Name]***Middleware*** <br>
* AuthorizationFilter
> 說明：最優先執行，通常驗證Request合不合法 <br>
> 命名規則：[Name]***AuthorizationFilter*** <br>
* ResourceFilter
> 說明：Model Binding前執行 <br>
> 命名規則：[Name]***ResourceFilter*** <br>
* ActionFilter
> 說明：Model Binding後執行 <br>
> 命名規則：[Name]***ActionFilter*** <br>
* ResultFilter
> 說明：Action完成後執行 <br>
> 命名規則：[Name]***ResultFilter*** <br>
* ExceptionFilter
> 說明：發生異常時執行 <br>
> 命名規則：[Name]***ExceptionFilter***  <br>
* Model
> 說明：資料結構 <br>
> 命名規則：[Name][Method][Input/Output]***Model*** <br>
## Batch 批次程式
專案相依：Domain、DataLayer、BusinessLayer <br>
分層結構 <br>
* Runner
> 服務進入點 <br>
> 命名規則：[Name]***Runner*** <br>
* Model
> 說明：資料結構 <br>
> 命名規則：[Name][Method][Input/Output]***Model*** <br>
* Mapper
> 說明：資料結構轉換 <br>
> 命名規則：[Name]***MapperConfiguration***  <br>
