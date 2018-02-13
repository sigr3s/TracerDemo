import * as nprogress from 'nprogress';
import { bindable, noView } from 'aurelia-framework';

@noView(['nprogress/nprogress.css'])
export class LoadingIndicator {
	@bindable loading = false;

	//https://github.com/rstacruz/nprogress
	loadingChanged(newValue) {
		if (newValue) {
			nprogress.configure({ showSpinner: false });
			nprogress.start();
		} else {
			nprogress.done();
		}
	}
}
