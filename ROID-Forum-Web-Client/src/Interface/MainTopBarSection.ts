import './MainTopBarSection.scss';

import {AppController} from "../AppController";
import {Interface} from "../Utility/Interface";
import {Section} from "./Section/Section";

export class MainTopBarSection {
    website: AppController;
    mainDiv: HTMLDivElement;
    titleDiv: HTMLDivElement;
    sectionsBarDiv: HTMLDivElement;
    sectionBarHighlight: HTMLDivElement;
    divSections: HTMLDivElement[] = [];
    widthPercentages: number[] = [];
    positionPercentages: number[] = [];

    constructor(w: AppController) {
        this.website = w;

        this.mainDiv = Interface.Create({type: 'div', className: 'TopBarSection', elements: [
                this.titleDiv = Interface.Create({type: 'div', className: 'TopBarTitle', text: 'MatthewFrench.io Forum'}),
                this.sectionsBarDiv = Interface.Create({type: 'div', className: 'SectionsBarDiv', elements: [
                        this.sectionBarHighlight = Interface.Create({type: 'div', className: 'Highlight'})
                    ]})
            ]});

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
            let s: HTMLDivElement = Interface.Create({type: 'div', className: 'SectionBarItem', onClick: () => {
                    this.sectionClick(i);
                }});
            s.innerText = sectionName;
            let width: number = 100.0 / totalLetters * sectionName.length;
            this.widthPercentages.push(width);
            this.positionPercentages.push(p);
            s.style.width = `${width}%`;
            s.style.left = `${p}%`;
            p += width;
            if (i != 0) {
                s.classList.add('NotFirst');
            }
            this.sectionsBarDiv.appendChild(s);
            this.divSections.push(s);
        }

        this.website.websiteDiv.appendChild(this.mainDiv);
    }

    darkTheme() {
        this.titleDiv.classList.add('DarkTheme');
        this.titleDiv.classList.remove('LightTheme');
        this.sectionsBarDiv.classList.add('DarkTheme');
        this.sectionsBarDiv.classList.remove('LightTheme');
    }

    lightTheme() {
        this.titleDiv.classList.remove('DarkTheme');
        this.titleDiv.classList.add('LightTheme');
        this.sectionsBarDiv.classList.remove('DarkTheme');
        this.sectionsBarDiv.classList.add('LightTheme');
    }

    logic() {}

    sectionClick(section: number) {
        for (let i = 0; i < this.divSections.length; i++) {
            let d: HTMLDivElement = this.divSections[i];
            d.classList.remove('Selected');
        }
        this.divSections[section].classList.add('Selected');
        this.website.showView(section);
        this.sectionBarHighlight.style.left = `${this.positionPercentages[section]}%`;
        this.sectionBarHighlight.style.width = `${this.widthPercentages[section]}%`;
    }
}