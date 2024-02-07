Source code of Project to change dns zone A record IP address of a website configured in cloudflare

- dns zone of website must be configured from cloudflare
- bearer token must be obtained from cloudflare account panel and updated over appsettings.json file
- website zone id can be checked via portal or from list zones api
- dns record id can be checked via portal or from list website dns zones


Can be useful when
- public ip address of router is changed frequently
- portforward is done via router in which we don't have admin access and need to update ip address of a record 
