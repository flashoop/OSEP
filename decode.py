import hashlib

def double_sha256(password: str) -> str:
    # Compute the first SHA256 hash
    first_hash = hashlib.sha256(password.encode()).digest()
    # Compute the second SHA256 hash
    second_hash = hashlib.sha256(first_hash).hexdigest()
    return second_hash

# Remove the "sha256" prefix from the provided hash
target_hash = "65ddcbd0b9afdb95304c63e8eb6326199657161adff54cc7b058699854b880222461f8922baa702b720d0486d3561b16a4fd2b4c8b4afba71e64405c1131fe485f69931f9f19e7138e92966c438b7061a7527ba77db4a01a51d68a9ee6530abdecdfecefade"

# Open a wordlist file and test each password
wordlist = "rockyou.txt"
with open(wordlist, "r", encoding="utf-8", errors="ignore") as f:
    for line in f:
        candidate = line.strip()
        if double_sha256(candidate) == target_hash:
            print(f"[+] Password found: {candidate}")
            break
    else:
        print("[-] Password not found.")

