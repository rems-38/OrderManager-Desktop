# Fichier de configuration

Pour fonctionner, cette application à besoin d'un fichier de configuration supplémentaire. Pour des raisons évidentes de sécurité, ce fichier n'est pas accesible dans ce dépôt GitHub mais voici ce qu'il doit contenir (remplacer les valeurs en fonction de votre configuration).

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<appSettings>
		<add key="server" value="ip_address_of_your_server"/> <!-- ex: 140.82.121.3-->
		<add key="port" value="3306"/>
		<add key="user" value="your_username"/>
		<add key="password" value="your_password"/>
		<add key="database" value="name_of_the_db"/>
	</appSettings>
</configuration>
```
Fichier _App.config_ à placer dans le dossier **OrderManager**.