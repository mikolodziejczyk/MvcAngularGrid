export class Debouncer {
    activeTimer: number | undefined = undefined;
    // set this to the callback method
    callback: () => void | null = null;
    timeout = 500;

    // call this on each input value change
    onChange() {
        if (this.activeTimer !== undefined) {
            window.clearTimeout(this.activeTimer);
        }

        this.activeTimer = window.setTimeout(() => {
            if (this.callback) {
                this.callback();
            }
        }, this.timeout);
    }
}
