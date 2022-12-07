
### generator curd

dotnet-aspnet-codegenerator controller -name GoodsCateController -m GoodsCateModel -dc MvcAndyContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries -sqlite


### migrat db
dotnet ef migrations add InitialComModel --context=MvcAndyContext
dotnet ef database update --context=MvcAndyContext