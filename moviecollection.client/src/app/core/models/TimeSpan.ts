const MILLIS_PER_SECOND = 1000;
const MILLIS_PER_MINUTE = MILLIS_PER_SECOND * 60;   //     60,000
const MILLIS_PER_HOUR = MILLIS_PER_MINUTE * 60;     //  3,600,000

export class TimeSpan {
    private _millis: number;

    private static round(n: number): number {
        if (n < 0) {
            return Math.ceil(n);
        } else if (n > 0) {
            return Math.floor(n);
        }

        return 0;
    }

    private static timeToMilliseconds(hour: number, minute: number): number {
        const totalSeconds = (hour * 3600) + (minute * 60);

        return totalSeconds * MILLIS_PER_SECOND;
    }

    public static fromTime(hours: number, minutes: number): TimeSpan {
        const millis = TimeSpan.timeToMilliseconds(hours, minutes);
        return new TimeSpan(millis);
    }

    constructor(millis: number) {
        this._millis = millis;
    }

    public get hours(): number {
        return TimeSpan.round((this._millis / MILLIS_PER_HOUR) % 24);
    }

    public get minutes(): number {
        return TimeSpan.round((this._millis / MILLIS_PER_MINUTE) % 60);
    }
}