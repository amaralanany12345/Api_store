# Project Name

Online store Api

## Features

- User registeration with Jwt token and Refresh token authentication
- category service
- item service
- mock payment gateway with idempotent pattern
- Email service using Brevo provider

## Packages
- dotnet restore

## Migrations
- there are two migration folders cause of two database , one for store and another for the wallet
  ### Wallet migration
    - add-migration addWalletToWalletDb -Context WalletAppDbcontext -outputdir Migrations/WalletMigrations
    - update-database -context WalletAppDbcontext
    - add-migration addWalletCurrency -Context WalletAppDbcontext -outputdir Migrations/WalletMigrations
    - update-database -context WalletAppDbContext
  ###
    - add-migration firstMigrationInAppDbContext -Context AppDbcontext -outputdir Migrations/StoreMigrations
    - update-database -context AppDbcontext
    - add-migration AddReceipt -Context AppDbcontext -outputdir Migrations/StoreMigrations
    - update-database -context AppDbcontext
    - add-migration updateOrderITemDeleteBehaviour -Context AppDbcontext -outputdir Migrations/StoreMigrations
    - update-database -context AppDbcontext
    - add-migration AddExternalLog -Context AppDbcontext -outputdir Migrations/StoreMigrations
    - update-database -context AppDbcontext

## Configurations
  - add data base ,jwt and brevo provider settings to the appsettings.json
    '''
      "Jwt": {
          "Audience": "http://localhost:5129",
          "Issuer": "http://localhost:5129",
          "DateTime": 10,
          "Signingkey": "GfjkoipsfgWQEY1234fdRfg45LOPhsFFF"
        },
        "ConnectionStrings": {
          "DefaultConnection": "Server=LAPTOP-NNDJ4G3D\\SQLEXPRESS; Database=onlineStore; Integrated Security=SSPI; TrustServerCertificate=True;",
          "WalletConnectionString": "Server=LAPTOP-NNDJ4G3D\\SQLEXPRESS; Database=WalletDb; Integrated Security=SSPI; TrustServerCertificate=True;"
        },
        "SmtpSettings": {
          "SmtpServer": "smtp-relay.brevo.com",
          "Port": 587,
          "Login": "aaeb9f001@smtp-brevo.com",
          "Password": "7NbjwILdKG1Cpvs2"
        }
    '''
## Assumptions
  - when you apply the configuration data of items, categories and users will upload by default to the database
  - when you register or sign in you will got in the response jwt token
  - you can take the token and put it in the Authorize button in the front of the swagger page
  - put the Bearer then your token and the authentication process will start  

## ERD diagram for store data base
![image_Alt](https://github.com/amaralanany12345/Api_store/blob/master/Screenshot%202026-05-14%20152644.png)

## ERD diagram for walletDb data base
![image_Alt](https://github.com/amaralanany12345/Api_store/blob/master/Screenshot%202026-05-14%20152707.png)
  
