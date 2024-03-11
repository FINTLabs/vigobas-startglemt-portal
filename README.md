# vigobas-startglemt-portal


## Description
Portal for activation of user accounts with forgot-password feature.


## Change log
Version | Description
------- | -------------
1.7.0   | Updated nuGet-packagesand  .NetFramework version

## Change log
Version | Description
------- | -------------
1.8.0   | Updated nuGet-packages
-	| Added Captcha feature. Can be set in web.config under appSettings
```

<appSettings>
...
...
	<add key="UseCaptchaActivateUser" value="true" />
	<add key="UseCaptchaForgottenUserName" value="true" />
	<add key="UseCaptchaForgottenPassword" value="true" />
...
...

```
