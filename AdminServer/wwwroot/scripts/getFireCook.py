import os

def get_firefox_netscape_cookies():
    firefox_cookie_path = os.path.join(os.getcwd(), 'wwwroot', 'cookies.sqlite')

    if not os.path.isfile(firefox_cookie_path):
        return 'Path not found'
    import sqlite3

    conn = sqlite3.connect(firefox_cookie_path)
    c = conn.cursor()

    c.execute("SELECT host, isSecure, path, expiry, name, value FROM moz_cookies")
    cookies = c.fetchall()

    netscape_cookies = []
    for cookie in cookies:
        domain = cookie[0]
        flag = str(bool(cookie[1])).upper()
        path = cookie[2]
        # secure = str(bool(cookie[3])).upper()
        expiration = cookie[3]
        name = cookie[4]
        value = cookie[5]
        # encrypted_value = chrome_password_decryption(cookie[6], chromeKey)

        netscape_cookie = f"{domain}\t{flag}\t{path}\tFALSE\t{expiration}\t{name}\t{value}"
        netscape_cookies.append(netscape_cookie)

    conn.close()

def main():
    firefox_cookies = get_firefox_netscape_cookies()
    with open(os.path.join(os.getcwd(), 'wwwroot', 'firefoxCookies.txt'), 'w') as file:
        file.write(firefox_cookies)

if __name__ == "__main__":
    main()