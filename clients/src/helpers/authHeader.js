
export function authHeader() {
    //return authorization with Json Web Token
    let user = JSON.parse(localStorage.getItem('user'));

    if (user && user.token){
        return {'Authorization' : 'Bearer' + user.token}
    } else {
        return {}
    }

}