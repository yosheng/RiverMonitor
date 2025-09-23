# 本地開發配置

## 資料庫配置
這邊使用 Podman 安裝 SQL Server 2022，先拉取
```shell
podman pull mcr.microsoft.com/mssql/server:2022-latest
```

使用 cmd 運行容器
```shell
podman run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=StrongP@ssw0rd!" ^
   --name "sql1" -p 1401:1433 ^
   -v sql1data:/var/opt/mssql ^
   -d mcr.microsoft.com/mssql/server:2022-latest
```