# Start- og glemtportal  

Start- og glemtportalen er en portal for aktivering av brukerkontoer, gjenoppretting av passord og utsending av brukernavn.

Portalen er lagd ved hjelp av ASP.NET MVC 5, og benytter seg av en tjeneste, ActivationCodes (som er en del av Users i IDMServices).

#### Konfigurasjon
For å kunne kjøre løsningen, må et endepunkt for tjenesten ActivationCodes (med tilhørende database) være tilgjengelig. Hvis denne kjører lokalt vil man typisk ha følgende konfigurasjon i Web.config:

```xml
<system.serviceModel>
    <bindings>
        <netTcpBinding>
        <binding name="NetTcpBinding_IActivationCodes" />
        </netTcpBinding>
    </bindings>
    <client>
        <endpoint address="net.tcp://localhost/Users/ActivationCodes.svc/soap" binding="netTcpBinding" bindingConfiguration="NetTcpBinding_IActivationCodes" contract="IDMServices.ActivationCodes.IActivationCodes" name="NetTcpBinding_IActivationCodes">
        </endpoint>
    </client>
</system.serviceModel>
```
##### Passordvalidering
I web.config ligger det en egen seksjon for konfigurering av passordvalideringsregler. Som standard følger det med 6 regler (et tall, en stor bokstav, en liten bokstav, et spesialtegn, 9 tegn, ikke æøå eller udefinerte tegn). Disse angis med en regex. 

```xml
<passwordValidation>
    <add name="HasNumbers" regex="[0-9]+" errorMessage="Passordet må inneholde et tall." description="Et tall" />
    <add name="HasUpperChar" regex="[A-Z]+" errorMessage="Passordet må ha minimum en stor bokstav." description="En stor bokstav" />
    <add name="HasLowerChar" regex="[a-z]+" errorMessage="Passordet må ha minimum en liten bokstav." description="En liten bokstav" />
    <add name="HasSpecialChar" regex="[!@#$%^*()_+=;:.?,-]+" errorMessage="Passordet må ha minimum et spesialtegn (!@#$%^*()_+=;:.?,-)." description="Et spesialtegn" />
    <add name="HasMinimumChars" regex=".{9,}" errorMessage="Passordet må bestå av minimum 9 tegn." description="9 tegn" />
    <add name="Whitelist" regex="^[a-zA-Z0-9!@#$%^*()_+=;:.?,-]*$" errorMessage="Passordet kan ikke ha æøå eller andre udefinerte tegn." description="Ikke æøå eller udefinerte tegn" />
  </passwordValidation>
```

Man kan legge til flere regler, eller fjerne (kommentere ut) eksisterende regler etter eget behov. Vær da oppmerksom på at både **name**, **regex**, **errorMessage** og **description** må være satt for at valideringen skal virke.

Ved endring av eksisterende regex, må man være oppmerksom på at visse spesialtegn må escapes i xml (".<>&). Ellers anbefales https://regex101.com/ for bygging og testing av regex.

#### Logging
Logging må konfigureres i egen fil, Nlog.config. Her angis blant annet mappen for hvor loggene lagres. Default er C:\TEMP\StartGlemt

***

#### Stiler

Alle endringer om farger og fonter kan gjøres via file style-settings.scss. Denne finnes i mappa Content>SCSS. Det er slik at man kan endre verdien på variablene for å tilpasse look-and-feel'en til en kommune sin grafisk profil.

```scss

// -------------------------------------------------- VARIABLES

// fonts

@import url('https://fonts.googleapis.com/css?family=Roboto:100,100i,300,300i,400,400i,500,500i,700,700i,900,900i');
@import url('https://fonts.googleapis.com/css?family=Roboto+Mono:100,300,400,500,700');

// logo

$logourl: "http://placehold.it/400x80/eeeeee/555555?text=LOGO";

// font families

$font-default: 'Roboto', Arial, sans-serif;
$font-mono: 'Roboto Mono', Consolas, monospace;

// font sizes

$fs-default: 16px;
$fs-xxs: 11px;
$fs-xs: 13px;
$fs-sm: 14px;
$fs-md: 20px;
$fs-lg: 28px;
$fs-xl: 32px;

// font weights

$fw-thin: 100;
$fw-light: 300;
$fw-regular: 400;
$fw-medium: 500;
$fw-bold: 700;
$fw-black: 900;

// support colors

$white: #fff;
$black: #333;
$dark-grey: #666;
$medium-grey: #888;
$light-grey: #e5e5e5;
$light-grey: #d7d7d7;


// layout colors

$primary-color: #7a1668;
$secondary-color: #513f35;
$feature-color1: #0b8797;
$feature-color2: #d0eaed;

$danger-color: #ff0000;
$success-color: #008000;

$alert-success: #8bd495;
$alert-success-border-color: #00a651;
$alert-fail: #e71924;
$alert-fail-border-color: #ed1c24;

// text colors

$text-color: $secondary-color;
$placeholder-color: #837067;

// frontpage icons url

$iconuser : "../images/icon-user.png";
$iconkey : "../images/icon-key.png";
$iconinfo : "../images/icon-info.png";
$iconuserhover : "../images/icon-user2.png";
$iconkeyhover: "../images/icon-key2.png";
$iconinfohover : "../images/icon-info2.png";

// media queries breakpoints (follows bootstrap)

$breakpoint-xs: 480px;
$breakpoint-sm: 768px;
$breakpoint-md: "992px";
$breakpoint-lg: 1200px;

```

#### LOGO

Logo har et placeholderbilde som default. Når man endre urlen i variable $logourl vil denne erstatte placeholderen. Verdiene på variabelen må være en sti, som en ekstern lenke sånn som det vises her, eller "../images/logo.svg", feks.

```
$logourl: "http://placehold.it/400x80/eeeeee/555555?text=LOGO"; 
```

Etter å ha utført endringer må man kompilere scss-filene med feks web compiler dersom man ikke har en compiler installert fra før av. Feks kan man bruker https://marketplace.visualstudio.com/items?itemName=MadsKristensen.WebCompiler, men det finnes mange andre måter å gjøre det på, feks i kommandolinja.

