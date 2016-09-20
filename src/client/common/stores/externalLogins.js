import { observable, computed, action } from 'mobx';
import api from 'helpers/api';

class ExternalLogins {
  // {
  //   scheme: 'Google',
  //   displayName: 'Google'
  // }
  @observable loginProviders = [];
}

export default ExternalLogins;