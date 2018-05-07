/**
 * Handles interface logic. Such as creation and displaying certain elements.
 */
export class Interface {
  /**
   * Recalculates an element so it can be animated by CSS transitions.
   */
  static ReflowElement(element : HTMLElement) {
    element.getBoundingClientRect();
  }

  /**
   * Allows nested creation of elements to manipulate the dom in a simple but powerful manner. All parameters are optional.
   */
  static Create({type = 'div', className = null, inputType = null, text = null, html = null, opacity = null,
                  appendTo = null, src = null, id = null, value = null, name = null, onClick = null, onMouseMove = null, onMouseDown = null,
                  onMouseUp = null, onMouseOver = null, onMouseOut = null, elements = [], placeholder = null, href = null,
                  target = null, onKeyUp = null, onKeyDown = null, colspan = null, onChange = null, onInput = null,
                  attributes = null, style = null, onFocus = null, outFocus = null, onBlur = null, width = null, height = null,
                  min = null, max = null, step = null, backgroundImage = null, title = null, checked = null, disabled = null,
                    valueAsDate = null, selected = null, onDoubleClick = null, draggable = null,
                  onDrop = null, onDragOver = null, onDragStart = null, onKeyPress = null
                } : {type? : string, className? : string, inputType? : string, text? : string, html? : string, opacity? : number,
      appendTo? : HTMLElement, src? : string, id? : string, value? : string, name? : string, onClick? : (event? : Event)=>void,
      onMouseMove? : (event? : Event)=>void, onMouseDown? : (event? : Event)=>void,
      onMouseUp? : (event? : Event)=>void, onMouseOver? : (event? : Event)=>void, onMouseOut? : (event? : Event)=>void, elements? : any[],
      placeholder? : string, href? : string,
      target? : HTMLElement, onKeyUp? : (event? : Event)=>void, onKeyDown? : (event? : Event)=>void, colspan? : string,
      onChange? : (event? : Event)=>void, onInput? : (event? : Event)=>void,
      attributes? : any, style? : any, onFocus? : (event? : Event)=>void, outFocus? : (event? : Event)=>void,
      onBlur? : (event? : Event)=>void, width? : number, height? : number,
      min? : number, max? : number, step? : number, backgroundImage? : string, title? : string, checked? : boolean, disabled? : boolean,
      valueAsDate? : Date, selected? : boolean, onDoubleClick? : (event? : Event)=>void, draggable? : boolean,
      onDrop? : (event? : Event)=>void, onDragOver? : (event? : Event)=>void, onDragStart? : (event? : Event)=>void,
      onKeyPress? : (event? : Event) => void
  } = {}) : any {
    //type : Element type to create
    //class : Class name of element
    //text : Inner text
    //html : Set inner html
    //appendTo : Append to element
    //elements : Inner elements to append to this element
    let element = document.createElement(type);
    let inputElement: any = element;

    if (className !== null) element.className = className;
    if (inputType !== null) {
        inputElement.type = inputType;
      if (text !== null) inputElement.value = text;
    } else {
      if (text !== null) {
        if (type.toLowerCase() === 'input') {
            inputElement.value = text;
        } else {
          element.textContent = text;
        }
      }
    }
    if (valueAsDate !== null) inputElement.valueAsDate = valueAsDate;
    if (html !== null) element.innerHTML = html;
    if (title !== null) element.title = title;
    if (checked !== null) inputElement.checked = checked;
    if (selected !== null) inputElement.selected = selected;
    if (disabled !== null) inputElement.disabled = disabled;
    if (opacity !== null) inputElement.style.opacity = opacity;
    if (appendTo !== null) appendTo.appendChild(element);
    if (src !== null) inputElement.src = src;
    if (id !== null) element.id = id;
    if (value !== null) inputElement.value = value;
    if (name !== null) inputElement.name = name;
    if (onClick !== null) element.onclick = onClick;
    if (onDoubleClick !== null) element.ondblclick = onDoubleClick;
    if (onMouseMove !== null) element.onmousemove = onMouseMove;
    if (onMouseDown !== null) element.onmousedown = onMouseDown;
    if (onMouseUp !== null) element.onmouseup = onMouseUp;
    if (onMouseOver !== null) element.onmouseover = onMouseOver;
    if (onMouseOut !== null) element.onmouseout = onMouseOut;
    if (onKeyUp !== null) element.onkeyup = onKeyUp;
    if (onKeyDown !== null) element.onkeydown = onKeyDown;
    if (onKeyPress !== null) element.onkeypress = onKeyPress;
    if (onInput !== null) element.oninput = onInput;
    if (onChange !== null) element.onchange = onChange;
    if (onFocus !== null) element.onfocus = onFocus;
    if (outFocus !== null) inputElement.onfocusout = outFocus;
    if (onBlur !== null) element.onblur = onBlur;
    if (placeholder !== null) inputElement.placeholder = placeholder;
    if (href !== null) inputElement.href = href;
    if (target !== null) inputElement.target = target;
    if (colspan !== null) inputElement.colSpan = colspan;
    if (width !== null) inputElement.width = width;
    if (height !== null) inputElement.height = height;
    if (min !== null) inputElement.min = min;
    if (max !== null) inputElement.max = max;
    if (step !== null) inputElement.step = step;
    if (backgroundImage !== null) inputElement.backgroundImage = backgroundImage;
    if (draggable !== null) element.draggable = draggable;
    if (onDrop !== null) element.ondrop = onDrop;
    if (onDragOver !== null) element.ondragover = onDragOver;
    if (onDragStart !== null) element.ondragstart = onDragStart;
    if (attributes !== null) {
      Object.keys(attributes).forEach((key) => {
        element.setAttribute(key, attributes[key]);
      });
    }
    if (style !== null) {
      Object.keys(style).forEach((key) => {
        element.style.setProperty(key, style[key]);
      });
    }

    for (let addElement of elements) {
      if (addElement instanceof Element) {
        element.appendChild(addElement);
      } else {
        element.appendChild(Interface.Create(addElement));
      }
    }

    return element;
  }
}