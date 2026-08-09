using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ABP.Core.Application.Interfaces;
using ABP.Core.Application.Dtos;
using System.Threading.Tasks;

namespace Artemis_Banking_Pro.Controllers
{
    [Authorize(Roles = "Cajero")]
    public class CajeroController : Controller
    {
        private readonly ICashierService _cashierService;
        private readonly ILogger<CajeroController> _logger;

        public CajeroController(ICashierService cashierService, ILogger<CajeroController> logger)
        {
            _cashierService = cashierService;
            _logger = logger;
        }

        // Dashboard ---------------------------------
        public async Task<IActionResult> Dashboard()
        {
            var dashboard = await _cashierService.GetDashboardAsync(User.Identity?.Name ?? "");
            return View(dashboard);
        }

        // Depósito -----------------------------------
        [HttpGet]
        public IActionResult Deposit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var id = await _cashierService.DepositAsync(dto);
            ViewBag.TransactionId = id;
            return View("Confirmation", "Depósito realizado con éxito.");
        }

        // Retiro -------------------------------------
        [HttpGet]
        public IActionResult Withdraw()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(WithdrawalDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var id = await _cashierService.WithdrawAsync(dto);
            ViewBag.TransactionId = id;
            return View("Confirmation", "Retiro realizado con éxito.");
        }

        // Pago Tarjeta Crédito -----------------------
        [HttpGet]
        public IActionResult PayCreditCard()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PayCreditCard(string creditCardNumber, decimal amount)
        {
            var id = await _cashierService.PayCreditCardAsync(creditCardNumber, amount);
            ViewBag.TransactionId = id;
            return View("Confirmation", "Pago con tarjeta de crédito exitoso.");
        }

        // Pago Préstamo ------------------------------
        [HttpGet]
        public IActionResult PayLoan()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PayLoan(string loanNumber, decimal amount)
        {
            var id = await _cashierService.PayLoanAsync(loanNumber, amount);
            ViewBag.TransactionId = id;
            return View("Confirmation", "Pago de préstamo exitoso.");
        }

        // Transferencia a terceros -------------------
        [HttpGet]
        public IActionResult TransferThirdParty()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TransferThirdParty(string destinationAccountNumber, decimal amount)
        {
            var id = await _cashierService.TransferToThirdPartyAsync(destinationAccountNumber, amount);
            ViewBag.TransactionId = id;
            return View("Confirmation", "Transferencia a terceros completada.");
        }

        // Historial ----------------------------------
        public async Task<IActionResult> History(int page = 1, int pageSize = 20)
        {
            var history = await _cashierService.GetTransactionHistoryAsync(User.Identity?.Name ?? "", page, pageSize);
            return View(history);
        }
    }
}
