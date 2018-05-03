//Load the CSS
//import style from './Main.scss';
import './Main.scss';

import {AppController} from "./AppController";

/**
 * Initializes the documents page.
 */
function main() {
  let app = new AppController();
  document.body.appendChild(app.getInterface());
}

//Initializes everything when the page is finished loading.
document.addEventListener('DOMContentLoaded', main);
