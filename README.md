# ASP.NET-Metrics
ASP.NET Core WebApi Metrics-Manager
## Сервис по сбору метрик

Включает Агентов сбора и Менеджер метрик.

Агенты собирают метрики CPU (% загруженности), метрики  GcHeapSize (байт во всех кучах(Heaps), метрики Hdd (Всего свободно Mбайт на дисках), метрики Network (Всего байт в секунду на сетевых адаптерах), метрики Ram (доступно Мбайт).

Менджер получает данные с агентов сбора метрик и накапливает в БД (SQLite). Впоследствии данные могут быть проанализированы за период времени, как с отдельного агента, так и с целого кластер

Реазиован Swagger UI (http://localhost:55557/index.html).
