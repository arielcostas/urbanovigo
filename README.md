# UrbanoVigo

## Wrapper en C# de la API del Concello

Se trata de un wrapper en C# de la API de datos del Concello de Vigo, usada principalmente por
sus apps oficiales. Esta API proporciona información sobre el transporte urbano de Vigo, entre otros,
sin depender de hacer scraping a la web de Vitrasa.

## Bot de Telegram

Un bot de Telegram que permite consultar el tiempo de espera de los autobuses de Vitrasa en Vigo.

Bot actualmente operativo en la cuenta [`@infobus_vigo_bot`](https://t.me/infobus_vigo_bot) de Telegram.

<img src="qr.jpg" alt="QR" width="120" />

### ¿Por qué?

1. Moovit no tiene apoyo oficial por parte de la empresa ni el Concello. Y tiene anuncios muy molestos (que hay que
   pagar por quitar).
2. Acordarse de los códigos de parada y entrar a Infobus[^1] por URL es un rollazo.
3. Vitrasa no da una API oficial que haría esto mucho más fácil e interoperable.

### Cómo funciona

El bot está programado en .NET 8 con C#, utilizando [Telegram.Bot](https://www.nuget.org/packages/Telegram.Bot/19.0.0) para interactuar con la API de Telegram.

- Para consultar el tiempo de espera de los autobuses de Vitrasa el bot consulta la API del Concello de Vigo y
  devuelve los datos en un mensaje de texto por Telegram. 
- Para buscar paradas por nombre el bot utiliza el dataset JSON del Concello de Vigo[^2] con las paradas de Vitrasa.
  Además, se utiliza [FuzzySharp](https://www.nuget.org/packages/FuzzySharp/2.0.2) para permitir mayor flexibilidad a
  las búsquedas.

## Licencia

Este proyecto está licenciado bajo la licencia BSD 3-Clause. Es una licencia muy sencilla de leer y con unas condiciones
más que razonables (dame crédito, no digas que apoyo tu proyecto y no me hago responsable si algo está roto). Puedes
leer la licencia completa en el archivo [LICENCE](LICENCE).

El contenido que devuelve el bot es propiedad de sus respectivos dueños, y no me hago responsable de que sea o no
exacto, o de los daños que pueda causar su uso. Los datos del Concello están bajo la Open Data Commons Attribution
License.

## Inspiración

- [Vitrasa Telegram Bot](https://github.com/dpeite/VitrasaTelegramBot)
- [Time for VBus API](https://github.com/abdonrd/time-for-vbus-api)

[^1]: `http://infobus.vitrasa.es:8002/Default.aspx?parada=` + código de parada
[^2]: [Dataset de paradas de Vitrasa](https://datos.vigo.org/data/transporte/paradas.json)