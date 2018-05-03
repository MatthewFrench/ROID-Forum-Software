import {Stopwatch} from "./Stopwatch";

/**
 * Handles very precises consistent looping.
 */
export class NanoTimer {
    isRunning : boolean;
    isLooping : boolean;
    consistencyStopwatch : Stopwatch;
    timeSinceLastStopwatch : Stopwatch;
    callback : (delta : number)=>{};
    milliseconds : number;
    constructor(callback : (delta : number)=>{}, milliseconds : number) {
        this.isRunning = false;
        this.isLooping = false;
        this.consistencyStopwatch = new Stopwatch();
        this.timeSinceLastStopwatch = new Stopwatch();
        this.callback = callback;
        this.milliseconds = milliseconds;
        this.consistencyStopwatch.reset();
        this.consistencyStopwatch.start();
        this.timeSinceLastStopwatch.reset();
        this.timeSinceLastStopwatch.start();
    }

    /**
     * Handles loop logic.
     */
    loop = () => {
        if (this.isLooping || !this.isRunning) {
            return;
        }
        this.isLooping = true;

        let elapsed = this.consistencyStopwatch.getMilliseconds();
        let timeUntilNextCallback = this.milliseconds - elapsed;
        if (timeUntilNextCallback <= 0) {
            let timeSinceLast = this.timeSinceLastStopwatch.getMilliseconds();
            this.timeSinceLastStopwatch.reset();
            this.timeSinceLastStopwatch.start();
            let delta = timeSinceLast / this.milliseconds;

            this.consistencyStopwatch.reset();
            this.consistencyStopwatch.addSeconds(Math.abs(timeUntilNextCallback) / 1000.0);
            this.consistencyStopwatch.start();
            this.callback(delta);
            elapsed = this.consistencyStopwatch.getMilliseconds();
            timeUntilNextCallback = this.milliseconds - elapsed;
        }

        this.isLooping = false;

        if (timeUntilNextCallback  <= 10) {
            if (timeUntilNextCallback <= 5) {
                if (timeUntilNextCallback <= 0.0) {
                    if (timeUntilNextCallback <= -this.milliseconds) {
                        this.loop();
                    } else {
                        //setImmediate(this.loop);
                        setTimeout(this.loop,0);
                    }
                } else {
                    setTimeout(this.loop,0);
                }
            } else {
                setTimeout(this.loop,1);
            }
        } else {
            setTimeout(this.loop,2);
        }
    };
    /**
     * Starts the high precision timer.
     */
    start = () => {
        if (this.isRunning) {
            return;
        }
        this.isRunning = true;
        this.consistencyStopwatch.reset();
        this.consistencyStopwatch.start();
        this.loop();
    };

    /**
     * Stops the high precision timer.
     */
    stop = () => {
        if (!this.isRunning) {
            return;
        }
        this.isRunning = false;
    };
}