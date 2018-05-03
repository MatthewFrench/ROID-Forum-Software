/**
 * Created by mattf on 7/5/2017.
 */


/**
 How to use WebHistory:

 (1) At the start of your website activate WebHistory by calling:
 WebHistory.Activate();

 (2) After UI creation, let WebHistory load any existing URL in the URL bar if there is one:
 WebHistory.LoadStartPageFromURL();

 (3) For every page switching action, push a state change, this enables the use of the back button:
 WebHistory.PushStateChange(['OutsidePage', 'InnerPage', 'Etc'], {dataToBeSaved: 5});

 (4) For every page, add a callback for loading the page from a WebHistory callback event(like back button being clicked):
 WebHistory.RegisterCallbackForStateChange(['OutsidePage', 'InnerPage', 'Etc'], loadOutsidePage_InnerPage_Etc);
 function loadOutsidePage_InnerPage_Etc(pageArray, data) {
    //Build forms and page details from saved state data.
 }

 */
export class WebHistory {
    private static allPageCallbacks : ( ( pageArray:string[], data:{} )=>void )[] = [];
    private static specificPageCallbacks : [string[], (pageArray:string[], data:{})=>void][] = [];
    private static startingPageLocation : string[] = [];

    /**
     * Activates the triggering of popstate.
     * @constructor
     */
    public static Activate = () => {
        window.addEventListener('popstate', WebHistory.OnPopStateTrigger);

        let hashLocation = window.location.hash;
        if (hashLocation.length > 0) {
            if (hashLocation.indexOf('#') == 0) {
                hashLocation = hashLocation.substring(1);
            }
            WebHistory.startingPageLocation = hashLocation.split('/');
        }
    };

    /**
     * Loads the page from the url, if the user has a #Page/Page/Page in the URL
     * @constructor
     */
    public static LoadStartPageFromURL = () => {
        if (WebHistory.startingPageLocation.length > 0) {
            WebHistory.TriggerCallbacksForPage(WebHistory.startingPageLocation, {});
            WebHistory.startingPageLocation = [];
        }
    };

    /**
     * Returns true if there is no starting URL that needs loaded
     * @returns {boolean}
     * @constructor
     */
    public static HasStartPage = () => {
        return WebHistory.startingPageLocation.length !== 0;
    };

    /**
     * When the website requests a hash location, all callbacks for any hash are triggered.
     */
    public static RegisterCallbackForAllStateChanges = (callback: ( pageArray:string[], data:{} )=>void) => {
        WebHistory.allPageCallbacks.push(callback);
    };

    /**
     * When the website requests a hash location, all callbacks registered to the specific hash are triggered.
     */
    public static RegisterCallbackForStateChange = (pageArray: string[], callback: ( pageArray:string[], data:{} )=>void) => {
        WebHistory.specificPageCallbacks.push([pageArray, callback]);
    };

    /**
     * Pushes a website state change into website history. Used mostly on page show function.
     */
    public static PushStateChange = (pageArray : string[], data : {} = {}) => {
        let currentState = history.state;
        let pageString : string = pageArray.join('/');
        if (currentState != null && currentState['pageArray'].join('/') != pageString) {
            history.pushState({pageArray: pageArray, data: data}, pageString, "#"+pageString);
        } else {
            WebHistory.UpdateStateData(pageArray, data);
        }
    };

    /**
     * Updates the data associated with a page. Used mostly on page hide function to save the state of the page.
     * @constructor
     */
    public static UpdateStateData = (pageArray : string[], data : {} = {}) => {
        let pageString : string = pageArray.join('/');
        history.replaceState({pageArray: pageArray, data: data}, pageString, "#"+pageString);
    };

    /**
     * Triggered when a state change occurs(user presses back button). Triggers callbacks.
     * @param event
     * @constructor
     */
    public static OnPopStateTrigger = (event : any) => {
        let stateData = event.state;
        let pageArray = [];
        let data = {};
        if (stateData != null) {
            pageArray = stateData['pageArray'];
            data = stateData['data'];
        }
        //Pull the location from the URL if it is not given.
        if (pageArray.length === 0) {
            let hashLocation = window.location.hash;
            if (hashLocation.length > 0) {
                if (hashLocation.indexOf('#') == 0) {
                    hashLocation = hashLocation.substring(1);
                }
                pageArray = hashLocation.split('/');
            }
        }

        WebHistory.TriggerCallbacksForPage(pageArray, data);
    };

    /**
     * Triggers all callbacks that are set for the specified page.
     * @param {string[]} pageArray
     * @param {{}} data
     * @constructor
     */
    public static TriggerCallbacksForPage = (pageArray : string[], data : {}) => {
        //Loop through all specific page callbacks
        for (let callbackItem of WebHistory.specificPageCallbacks) {
            let callbackPageArray = callbackItem[0];
            let runCallback = false;
            //Page array is in format /page1/page2/page3
            if (callbackPageArray.length <= pageArray.length) {
                //If callback page exists in pageArray, then call the callback
                let isMatching = true;
                for (let pageIndex = 0; pageIndex < callbackPageArray.length; pageIndex++) {
                    let pageSection = callbackPageArray[pageIndex];
                    if (pageSection != pageArray[pageIndex]) {
                        isMatching = false;
                    }
                }
                if (isMatching) {
                    runCallback = true;
                }
            }
            if (runCallback) {
                callbackItem[1](pageArray, data);
            }
        }
        for (let callbackItem of WebHistory.allPageCallbacks) {
            callbackItem(pageArray, data);
        }
    };
}