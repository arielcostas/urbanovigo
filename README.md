# Infobus Bot (Telegram)

Un bot de Telegram que permite consultar o tempo de espera dos autobuses de Vitrasa en Vigo.

Bot actualmente operativo na conta [`@infobus_vigo_bot`](https://t.me/infobus_vigo_bot) de Telegram.

<img src="qr.jpg" alt="QR" width="120" />

## Por qué?

1. Moovit no tiene soporte oficial por la empresa ni el Concello. Y tiene anuncios muy molestos (que hay que pagar por
   quitar).
2. Acordarse de los códigos de parada y entrar a Infobus[^1] por URL
   es un rollazo.
3. Vitrasa no da una API oficial que haría esto mucho más fácil e interoperable.
4. Porque puedo.

## Cómo funciona

De primeiras, o bot non ten base de datos nin nada. Está programado en .NET 7 con C# e utiliza as bibliotecas listadas
no arquivo [BotVitrasa.csproj](BotVitrasa.csproj). O bot execútase nun servidor Linux como servizo do sistema.

- Para consultar o tempo de espera dos autobuses de Vitrasa o bot fai unha petición a Infobus, escanea o HTML e
  retorna os resultados en texto por mensaxe de Telegram.
- Para buscar paradas por nome o bot utiliza o dataset JSON do Concello de Vigo[^2] coas paradas de Vitrasa.

## Funcionalidades

- [X] Consultar o tempo estimado de chegada dos autobuses de Vitrasa nunha parada concreta.
- [X] Buscar paradas por nome.
- [ ] Buscar paradas por ubicación.
- [ ] Interfaz con botóns para facilitar a interacción co bot.
- [ ] Aviso cando se publiquen anuncios en [vitrasa.es](https://vitrasa.es).
- [ ] Facer un logo en condicións, e quizás outro nombre.
- [ ] Crear unha "caché" das estimacións para non facer máis dunha 1 petición por parada/minuto.
- [ ] Soporte para outras plataformas de mensaxería.

## Licencia

Este proxecto está licenciado baijo la licencia BSD 3-Clause. É unha licencia moi sinxela de ler e cunhas condiciones
máis que razoables (dame crédito, non digas que apoio o teu proyecto derivado e non me fago responsable se algo está
roto). Podes ler a licencia completa no arquivo [LICENCE](LICENCE).

O contido que devolve o bot é propiedade dos seis respectivos donos, e non me fago responsable de que sexa ou no exacto,
ou dos daños que poda causar o seu uso. Os datos do Concello están baixo a Open Data Commons Attribution License.

## Inspiración

- [Vitrasa Telegram Bot](https://github.com/dpeite/VitrasaTelegramBot)
- [Time for VBus API](https://github.com/abdonrd/time-for-vbus-api)

[^1]: `http://infobus.vitrasa.es:8002/Default.aspx?parada=` + código de parada
[^2]: [Dataset de paradas de Vitrasa](https://datos.vigo.org/data/transporte/paradas.json)