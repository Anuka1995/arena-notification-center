import React from 'react';
import ReactDOM from 'react-dom';
import { IntlProvider } from "react-intl";

import App from './App';
import * as serviceWorker from './serviceWorker';
import localeData from "../src/messages/locales/data.json";
import { BrowserRouter } from 'react-router-dom';

import './index.css';

const language =
    (navigator.languages && navigator.languages[0]) ||
    navigator.language ||
    navigator.userLanguage;

// Split locales with a region code
const languageWithoutRegionCode = language.toLowerCase().split(/[_-]+/)[0];

// Try full locale, try locale without region code, fallback to 'en'
const messages =
    localeData[languageWithoutRegionCode] ||
    localeData[language] ||
    localeData.en;

ReactDOM.render(
    <IntlProvider locale={language} messages={messages}>
        <BrowserRouter><App /></BrowserRouter>
    </IntlProvider>, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
