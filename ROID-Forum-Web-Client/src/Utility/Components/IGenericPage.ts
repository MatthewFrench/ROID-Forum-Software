/**
 * Generic page interface for templating.
 */
export interface IGenericPage {
    showPage() : void;
    hidePage() : void;
    getInterface() : HTMLElement;
}