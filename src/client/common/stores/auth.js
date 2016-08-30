import { observable } from 'mobx';
import api from 'helpers/api';

export default class AuthStore {
    @observable user = null;

    @action
    login(username, password) {
        api.login(username, password).then(result => {
            console.log(result);
        });
    }
}
