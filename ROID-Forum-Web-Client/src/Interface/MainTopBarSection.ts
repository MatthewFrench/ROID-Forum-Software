import {AppController} from "../AppController";
import {Interface} from "../Utility/Interface";
import {Section} from "./CodingSection/Section";

export class MainTopBarSection {
    website: AppController;
    mainDiv: HTMLDivElement;
    titleDiv: HTMLDivElement;
    sectionsBarDiv: HTMLDivElement;
    sectionBarHighlight: HTMLDivElement;
    divSections: HTMLDivElement[] = [];
    widthPercentages: number[] = [];
    positionPercentages: number[] = [];
    currentHighlightWidth = 0.0;
    currentHighlightPosition = 0.0;
    newHighlightWidth = 0.0;
    newHighlightPosition = 0.0;
    hightlightMoveSpeed = 0.4;

    constructor(w: AppController) {
        this.website = w;
        this.makeMainDivComponent();
        this.makeGMGTitleComponent();
        this.makeSectionBarComponent();
        this.makeSectionBarHighlightComponent();
        this.addSections();
        this.website.websiteDiv.appendChild(this.mainDiv);
    }

    darkTheme() {
        this.titleDiv.style.color = "white";
        this.sectionsBarDiv.style.color = "white";
    }

    lightTheme() {
        this.titleDiv.style.color = "Black";
        this.sectionsBarDiv.style.color = "Black";
    }

    logic() {
        if (this.currentHighlightWidth != this.newHighlightWidth) {
            let movementSpeed = Math.abs((this.newHighlightWidth - this.currentHighlightWidth) / 5.0);
            if (movementSpeed < this.hightlightMoveSpeed) {
                movementSpeed = this.hightlightMoveSpeed;
            }
            if (this.currentHighlightWidth < this.newHighlightWidth) {
                this.currentHighlightWidth += movementSpeed;
            }
            if (this.currentHighlightWidth > this.newHighlightWidth) {
                this.currentHighlightWidth -= movementSpeed;
            }
            if (Math.abs(this.currentHighlightWidth - this.newHighlightWidth) < movementSpeed) {
                this.currentHighlightWidth = this.newHighlightWidth;
            }
            this.sectionBarHighlight.style.width = `${this.currentHighlightWidth}%`;
        }
        if (this.currentHighlightPosition != this.newHighlightPosition) {
            let movementSpeed = Math.abs((this.newHighlightPosition - this.currentHighlightPosition) / 5.0);
            if (movementSpeed < this.hightlightMoveSpeed) {
                movementSpeed = this.hightlightMoveSpeed;
            }
            if (this.currentHighlightPosition < this.newHighlightPosition) {
                this.currentHighlightPosition += movementSpeed;
            }
            if (this.currentHighlightPosition > this.newHighlightPosition) {
                this.currentHighlightPosition -= movementSpeed;
            }
            if (Math.abs(this.currentHighlightPosition - this.newHighlightPosition) < movementSpeed) {
                this.currentHighlightPosition = this.newHighlightPosition;
            }
            this.sectionBarHighlight.style.left = `${this.currentHighlightPosition}%`;
        }
    }

    sectionClick(section: number) {
        for (let i = 0; i < this.divSections.length; i++) {
            let d: HTMLDivElement = this.divSections[i];
            d.style.textDecoration = '';
        }
        this.divSections[section].style.textDecoration = 'underline';
        this.website.showView(section);
        this.newHighlightWidth = this.widthPercentages[section];
        this.newHighlightPosition = this.positionPercentages[section];
    }

    /*********** Create GUI Components *************/

    makeMainDivComponent() {
        this.mainDiv = Interface.Create({type: 'div'});
        this.mainDiv.style.position = "absolute";
        this.mainDiv.style.left = "0px";
        this.mainDiv.style.top = "0px";
        this.mainDiv.style.height = "92px";
        this.mainDiv.style.width = "calc(100% - 300px)";

    }

    makeGMGTitleComponent() {
        //Add the GMG name to the main div
        this.titleDiv = Interface.Create({type: 'div'});
        this.titleDiv.style.position = "absolute";
        this.titleDiv.style.left = "0px";
        this.titleDiv.style.top = "20px";
        this.titleDiv.style.width = "100%";
        this.titleDiv.style.fontSize = "15px";
        this.titleDiv.style.textAlign = "center";
        this.titleDiv.innerText = 'Game Maker\'s Garage';
        this.mainDiv.appendChild(this.titleDiv);
    }

    makeSectionBarComponent() {
        //Add the section bar
        this.sectionsBarDiv = Interface.Create({type: 'div'});
        this.sectionsBarDiv.style.position = "absolute";
        this.sectionsBarDiv.style.width = "80%";
        this.sectionsBarDiv.style.left = "10%";
        this.sectionsBarDiv.style.top = "50px";
        this.sectionsBarDiv.style.height = "40px";
        this.sectionsBarDiv.style.borderTopStyle = "solid";
        this.sectionsBarDiv.style.borderBottomStyle = "solid";
        this.sectionsBarDiv.style.borderWidth = "1px";
        this.sectionsBarDiv.style.borderColor = "#DDDDDD";
        this.sectionsBarDiv.style.userSelect = "none";
        this.mainDiv.appendChild(this.sectionsBarDiv);
    }

    addSections() {
        //Add the sections to the bar
        let totalLetters = 0;
        for (let i = 0; i < this.website.sectionOrder.length; i++) {
            let section: Section = this.website.sectionOrder[i];
            totalLetters += section.getDisplayName().length;
        }
        let p = 0.0;
        for (let i = 0; i < this.website.sectionOrder.length; i++) {
            let section: Section = this.website.sectionOrder[i];
            let sectionName: string = section.getDisplayName();
            let s: HTMLDivElement = Interface.Create({type: 'div'});
            s.innerText = sectionName;
            let width: number = 100.0 / totalLetters * sectionName.length;
            this.widthPercentages.push(width);
            this.positionPercentages.push(p);
            s.style.position = 'absolute';
            s.style.width = `${width}%`;
            s.style.textAlign = 'center';
            s.style.height = '40px';
            s.style.top = '0px';
            s.style.left = `${p}%`;
            s.style.lineHeight = '40px';
            s.style.textOverflow = 'ellipsis';
            s.style.whiteSpace = 'nowrap';
            s.style.overflow = 'hidden';
            s.style.cursor = 'pointer';
            s.onclick = () => {
                this.sectionClick(i);
            };
            p += width;
            if (i != 0) {
                s.style.borderLeft = 'solid';
                s.style.borderColor = '#DDDDDD';
                s.style.borderWidth = '1px';
            }
            this.sectionsBarDiv.appendChild(s);
            this.divSections.push(s);

        }
    }

    makeSectionBarHighlightComponent() {
        this.sectionBarHighlight = Interface.Create({type: 'div'});
        this.sectionBarHighlight.style.position = "absolute";
        this.sectionBarHighlight.style.top = "0px";
        this.sectionBarHighlight.style.left = "0px";
        this.sectionBarHighlight.style.height = "100%";
        this.sectionBarHighlight.style.width = "0%";
        this.sectionBarHighlight.style.backgroundColor = "rgba(100,200,100,0.5)";
        this.sectionsBarDiv.appendChild(this.sectionBarHighlight);
    }
}
