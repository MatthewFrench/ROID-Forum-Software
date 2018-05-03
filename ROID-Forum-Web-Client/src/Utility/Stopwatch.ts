/**
 * Created by matt on 10/7/16.
 */

/**
 * Handles precise tracking of time.
 */
const MS_PER_SEC = 1000;
const MS_PER_MIN = 60000;
const MS_PER_HOUR = 3.6e+6;
export class Stopwatch {
    timeArray : any[];
    startTime : number;
    constructor(startSeconds = 0) {
        //Holds an array of time intervals that have previously existed. Fills up by pausing.
        this.timeArray = [];
        //The latest current time of the stopwatch
        this.startTime = null;
        this.reset(startSeconds);
    }

    /**
     * Returns true if the stopwatch is paused.
     * @returns {boolean}
     */
    isPaused() {
        return this.startTime === null;
    }

    /**
     * Clears recorded time.
     */
    clearTime() {
        this.timeArray = [];
    }

    /**
     * Resets the stopwatch tracking.
     */
    reset(startSeconds = 0) {
        this.clearTime();
        if (startSeconds !== 0) {
            this.addSeconds(startSeconds);
        }
        this.start();
    }

    /**
     * Sets the seconds in the stopwatch.
     * @param seconds
     */
    setSeconds(seconds : number) {
        this.clearTime();
        this.addSeconds(seconds);
    }

    /**
     * Pushed a time interval into the array and stops the start time.
     */
    pause() {
        this.timeArray.push(this.getMillisecondsInCurrentInterval());
        this.startTime = null;
    }

    /**
     * Starts the start time, continues if the stopwatch wasn't reset.
     */
    start() {
        this.startTime = window.performance.now();
    }

    /**
     * Adds seconds to the timer
     */
    addSeconds(seconds : number) {
        this.timeArray.push(seconds * MS_PER_SEC);
    }

    /**
     * Returns only the milliseconds since the start interval.
     * @returns {number}
     */
    getMillisecondsInCurrentInterval() {
        if (this.startTime === null) {
            return 0;
        }
        return window.performance.now() - this.startTime;
    }

    /**
     * Returns the total milliseconds that has passed.
     * @returns {number}
     */
    getMilliseconds() {
        //Small optimization to quickly return information
        if (this.timeArray.length === 0) {
            if (this.startTime !== null) {
                return window.performance.now() - this.startTime;
            } else {
                return 0;
            }
        }
        let milliseconds = 0;
        for (let time of this.timeArray) {
            milliseconds += time;
        }
        milliseconds += this.getMillisecondsInCurrentInterval();
        return milliseconds;
    }

    /**
     * Returns the total seconds that has passed.
     * @returns {number}
     */
    getSeconds() {
        return this.getMilliseconds() / MS_PER_SEC;
    }

    /**
     * Returns the total minutes that has passed.
     * @returns {number}
     */
    getMinutes() {
        return this.getMilliseconds() / MS_PER_MIN;
    }

    /**
     * Returns the total hours that has passed.
     * @returns {number}
     */
    getHours() {
        return this.getMilliseconds() / MS_PER_HOUR;
    }
}