using Microsoft.AspNetCore.Identity;

namespace ElsWebApp.Areas.Identity.Models
{
    public class IdentityErrorDescriberJP : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() { return new IdentityError { Code = nameof(DefaultError), Description = $"不明な障害が発生しました。" }; }
        public override IdentityError ConcurrencyFailure() { return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "オプティミスティック同時実行エラー。オブジェクトが変更されました。" }; }
        public override IdentityError PasswordMismatch() { return new IdentityError { Code = nameof(PasswordMismatch), Description = "パスワードが間違っています。" }; }
        public override IdentityError InvalidToken() { return new IdentityError { Code = nameof(InvalidToken), Description = "無効なトークン。" }; }
        public override IdentityError LoginAlreadyAssociated() { return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "このログインを持つユーザーはすでに存在します。" }; }
        public override IdentityError InvalidUserName(string userName) { return new IdentityError { Code = nameof(InvalidUserName), Description = $"ユーザ名 '{userName}' は無効です。文字または数字のみを含めることができます。" }; }
        public override IdentityError InvalidEmail(string email) { return new IdentityError { Code = nameof(InvalidEmail), Description = $"Email '{email}' is invalid." }; }
        public override IdentityError DuplicateUserName(string userName) { return new IdentityError { Code = nameof(DuplicateUserName), Description = $"ユーザ名 '{userName}' は、既に使用されています。" }; }
        public override IdentityError DuplicateEmail(string email) { return new IdentityError { Code = nameof(DuplicateEmail), Description = $"メールアドレス '{email}' は、既に使用されています。" }; }
        public override IdentityError InvalidRoleName(string role) { return new IdentityError { Code = nameof(InvalidRoleName), Description = $"ロール名 '{role}' は無効です。" }; }
        public override IdentityError DuplicateRoleName(string role) { return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"ロール名 '{role}' は、既に使用されています。" }; }
        public override IdentityError UserAlreadyHasPassword() { return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "ユーザーは既にパスワードを設定しています。" }; }
        public override IdentityError UserLockoutNotEnabled() { return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "このユーザーに対してロックアウトは有効になっていません。" }; }
        public override IdentityError UserAlreadyInRole(string role) { return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"ユーザーはすでにロールを持っています '{role}'。" }; }
        public override IdentityError UserNotInRole(string role) { return new IdentityError { Code = nameof(UserNotInRole), Description = $"ユーザーはロールに属していません'{role}'." }; }
        public override IdentityError PasswordTooShort(int length) { return new IdentityError { Code = nameof(PasswordTooShort), Description = $"パスワードは少なくとも {length} 文字である必要があります。" }; }
        public override IdentityError PasswordRequiresNonAlphanumeric() { return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "パスワードには少なくとも 1 つの英数字以外の文字が含まれている必要があります。" }; }
        public override IdentityError PasswordRequiresDigit() { return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "パスワードには少なくとも 1 つの数字 (0～9) が必要です。」" }; }
        public override IdentityError PasswordRequiresLower() { return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "パスワードには少なくとも 1 つの小文字 (a～z) が必要です。" }; }
        public override IdentityError PasswordRequiresUpper() { return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "パスワードには少なくとも 1 つの大文字 (A～Z) が含まれている必要があります。" }; }
    }
}
