using Application.Common.Interfaces;

namespace Application.Queries;

public record IsValidMagicLinkTokenQuery(string Token, string TokenType) : IRequest<bool>;

public class IsValidMagicLinkTokenQueryHandler : IRequestHandler<IsValidMagicLinkTokenQuery, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public IsValidMagicLinkTokenQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(IsValidMagicLinkTokenQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Token)) return default;

        var tokenFoundInDb = await _unitOfWork.MagicLinkRepository.
            SingleOrDefaultAsync(x => x.Token == request.Token
                                      && x.MagicLinkType!.Name == request.TokenType,
                "MagicLinkType", false, cancellationToken);

        return tokenFoundInDb != null;
    }
}