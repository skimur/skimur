
// this decorator converts a fetch promise into another promise
// that plays well with model state
const apiCall = function(url) {
  return (target, key, descriptor) => {
    return {
      enumerable: false,
      configurable: true,
      value: function(...args) {
        var result = descriptor.value(...args);
        var fetchOptions = {
          method: 'POST',
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
          },
          credentials: 'include'
        };
        if(result) {
          fetchOptions.body = JSON.stringify(result);
        }
        var fetchPromise = fetch(url, fetchOptions);

        return new Promise(function(resolve, reject) {
          fetchPromise
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
}

class Api {
  @apiCall('/api/account/login')
  logIn(userName, password, rememberMe) {
    return {
      userName,
      password,
      rememberMe
    };
  }
  @apiCall('/api/account/logoff')
  logOff() {
    
  }
  @apiCall('/api/account/register')
  register(userName, email, password, passwordConfirm) {
    return {
      userName,
      email,
      password,
      passwordConfirm
    }
  }
  @apiCall('/api/account/sendcode')
  sendCode(provider) {
    return {
      provider
    };
  }
  @apiCall('/api/account/verifycode')
  verifyCode(provider, code, rememberMe, rememberBrowser) {
    return {
      provider,
      code,
      rememberMe,
      rememberBrowser
    };
  }
  @apiCall('/api/manage/security')
  getSecurity(provider, code, rememberMe, rememberBrowser) {
  }
  @apiCall('/api/manage/settwofactor')
  setTwoFactor(enabled) {
    return {
      enabled
    };
  }
  @apiCall('/api/manage/email')
  getEmail() {
  }
  @apiCall('/api/manage/changeemail')
  changeEmail(email, emailConfirm, currentPassword) {
    return {
      email,
      emailConfirm,
      currentPassword
    };
  }
  @apiCall('/api/manage/verifyemail')
  verifyEmail() {
  }
}

export default new Api();