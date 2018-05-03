import {Interface} from "../Interface";

/**
 * Tab bar logic and interface.
 */
export class TabBar {
    mainInterface : HTMLDivElement;
    itemValues : any[];
    items : HTMLElement[];
    itemCallbacks : (()=>void)[];
    selectedItem : HTMLElement;
    constructor(className = '') {
        this.mainInterface = Interface.Create({type: 'div', className: 'TabBar'});
        if (className.length > 0) {
            this.mainInterface.classList.add(className);
        }
        this.itemValues = [];
        this.items = [];
        this.itemCallbacks = [];
        this.selectedItem = null;
    }

    /**
     * Adds an item to the tab bar.
     * @param value
     * @param {string} name
     * @param {() => any} onClick
     * @param {string} className
     * @param {boolean} disabled
     */
    addItem({value = null, name, onClick = () => {}, className = '', disabled = false} :
                {value? : any, name: string, onClick?:()=>void, className?: string, disabled?: boolean}) {
        if (value === null) {
            value = name;
        }
        let callback = ()=>{this.itemClicked(item);};
        if (disabled) {
            callback = () => {};
        }
        let item = Interface.Create({type: 'div', className: 'TabItem', text: name, onClick: callback});
        if (className.length > 0) {
            item.classList.add(className);
        }
        if (disabled) {
            item.classList.add('Disabled');
        }
        this.itemValues.push(value);
        this.items.push(item);
        this.itemCallbacks.push(onClick);
        this.mainInterface.appendChild(item);
    }

    /**
     * Event for when an item is clicked in the tab bar.
     * @param {HTMLElement} item
     */
    itemClicked = (item : HTMLElement) => {
        if (item === this.selectedItem) {
            return;
        }
        if (this.selectedItem !== null) {
            this.selectedItem.classList.remove('Selected');
        }
        this.selectedItem = item;
        this.selectedItem.classList.add('Selected');
        let index = this.items.indexOf(item);
        let callback = this.itemCallbacks[index];
        callback();
    };

    /**
     * Sets the selected tab bar item.
     * @param {number} selectIndex
     */
    setSelectedItemByIndex(selectIndex : number) {
        this.itemClicked(this.items[selectIndex]);
    }

    /**
     * Sets the selected tab bar item.
     * @param  value
     */
    setSelectedItemByValue(value : any) {
        this.itemClicked(this.items[this.itemValues.indexOf(value)]);
    }

    /**
     * Gets the interface for the tab bar.
     * @returns {HTMLDivElement}
     */
    getInterface() {
        return this.mainInterface;
    }
}