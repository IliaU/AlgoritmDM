# AlgoritmDM

Разрабатывалось на Visual Studion 2015

Ключ реестра: HKEY_LOCAL_MACHINE\SOFTWARE\ORACLE\KEY_ODS12gr1 
переменная "NLS_LANG" типа REG_SZ 
на сервере клиента имела значение = "AMERICAN_AMERICA.WE8MSWIN1252"
я заменил на "RUSSIAN_RUSSIA.CL8MSWIN1251" - так было у нас на 9-ке

На 9-ке у нас стоит более старая версия Retail Pro 9, по этому на 9-ке путь чуть другой (по версии Oracle'а): HKEY_LOCAL_MACHINE\SOFTWARE\ORACLE\KEY_ODS11gr1

Для версии под призм необходимо скачать драйвера
https://dev.mysql.com/downloads/connector/odbc/