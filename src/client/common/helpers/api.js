
// this decorator converts a fetch promise into another promise
// that plays well with model state
const modelStatePromise = function(target, key, descriptor) {
    return {
      enumerable: false,
      configurable: true,
      value: function(...args) {
        var result = descriptor.value(...args);
        return new Promise(function(resolve, reject) {
          result
            .then(function(response) {
              // non-20x responses are errors
              if (response.status >= 200 && response.status < 300) {
                return response
              } else {
                var error = new Error(response.statusText)
                error.response = response
                throw error
              }
            })
            .then(function(result) {
              // successful response, just send it to ther user
              resolve(result.json());
            })
            .catch(function(ex) {
              // all exceptions get resolved normally,
              // but with a return type that mimicks modelstate
              resolve({
                "success": false,
                "errors": {
                  "_global": [ex.message]
                }
              });
            });
        });
      }
    }
}

class Api {
  @modelStatePromise
  logIn(userName, password, rememberMe) {
    return fetch('/api/account/login', {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        userName,
        password,
        rememberMe
      }),
      credentials: 'include'
    });
  }
  @modelStatePromise
  logOff() {
    return fetch('/api/account/logoff', {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      credentials: 'include'
    });
  }
  @modelStatePromise
  register(userName, email, password, passwordConfirm) {
    return fetch('/api/account/register', {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        userName,
        email,
        password,
        passwordConfirm
      }),
      credentials: 'include'
    });
  }
  @modelStatePromise
  sendCode(provider) {
    return fetch('/api/account/sendcode', {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        provider
      }),
      credentials: 'include'
    });
  }
  @modelStatePromise
  verifyCode(provider, code, rememberMe, rememberBrowser) {
    return fetch('/api/account/verifycode', {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        provider,
        code,
        rememberMe,
        rememberBrowser
      }),
      credentials: 'include'
    });
  }
}

export default new Api();