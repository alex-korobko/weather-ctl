# AngularJS based control to show weather for a selected city

## Overview
A tool that shows information about the weather in selected city.

The latest version could be found on https://github.com/alex-korobko/

## Prerequisites

### Git

- A good place to learn about setting up git is [here][git-github].
- Git [home][git-home] (download, documentation).

### Node.js and Tools

- Get [Node.js][node-download].
- Install the tool dependencies (`npm install`).


## Workings of the application
The application contains server and client side. 
The server side is a simple asp.net web.api server that supports RESTful model
The client side is an AngularJS 1.4 based control written using CommonJS style with Browerify (https://blog.codecentric.de/en/2014/08/angularjs-browserify/)


### Installing dependencies

The application relies upon various node.js tools, such as Gulp, Bower, Karma and Protractor.  You can
install these by running:

```
npm install
```

### Running the app during development

- Run `npm start`
- navigate your browser to `http://localhost:8000/app/index.html` to see the app running in your browser.

### Running unit tests

- Start Karma with `npm test`
  - A browser will start and connect to the Karma server. Chrome is the default browser, others can
  be captured by loading the same url as the one in Chrome or by changing the `test/karma.conf.js`
  file.
- Karma will sit and watch your application and test JavaScript files. To run or re-run tests just
  change any of your these files.


### End to end testing

Requires a webserver that serves the application. See Running the app during development, above.

- Serve the application: run `npm start`.
- In a separate console run the end2end tests: `npm run protractor`. Protractor will execute the
  end2end test scripts against the web application itself.
  - The configuration is set up to run the tests on Chrome directly. If you want to run against
    other browsers then you must install the webDriver, `npm run update-webdriver`, and modify the
  configuration at `test/protractor-conf.js`.
