var buffer = [UInt8](repeating: 0, count: 32)
_ = SecRandomCopyBytes(kSecRandomDefault, buffer.count, &buffer)
let verifier = Data(bytes: buffer).base64EncodedString()
.replacingOccurrences(of: "+", with: "-")
.replacingOccurrences(of: "/", with: "_")
.replacingOccurrences(of: "=", with: "")
.trimmingCharacters(in: .whitespaces)

// You need to import CommonCrypto
guard let data = verifier.data(using: .utf8) else { return nil }
var buffer = [UInt8](repeating: 0,  count: Int(CC_SHA256_DIGEST_LENGTH))
data.withUnsafeBytes {
_ = CC_SHA256($0, CC_LONG(data.count), &buffer)
}
let hash = Data(bytes: buffer)
let challenge = hash.base64EncodedString()
.replacingOccurrences(of: "+", with: "-")
.replacingOccurrences(of: "/", with: "_")
.replacingOccurrences(of: "=", with: "")
.trimmingCharacters(in: .whitespaces)