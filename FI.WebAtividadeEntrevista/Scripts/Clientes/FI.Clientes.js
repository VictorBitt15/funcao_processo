
$(document).ready(function () {

    $('#CPF,#BeneficiarioCPF').inputmask('999.999.999-99', {
        clearIncomplete: true,
        showMaskOnHover: false
    });

    $('#formCadastro').submit(function (e) {
        e.preventDefault();

        const $form = $(this);
        const dadosCliente = obterDadosCliente ($form);
        const beneficiarios = obterBeneficiarios();

        $.ajax({
            url: urlPost,
            method: "POST",
            data: {
                ...dadosCliente,
                Beneficiarios:beneficiarios
            },
            error:
                function (r) {
                    if (r.status == 400)
                        ModalDialog("Ocorreu um erro", r.responseJSON);
                    else if (r.status == 500)
                        ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
                },
            success:
                function (r) {
                    ModalDialog("Sucesso!", r)
                    $("#formCadastro")[0].reset();
                }
        });
    })

    $('#incluirBenefCliente').on('click', function () {
        const cpf = $('#BeneficiarioCPF').val();
        const nome = $('#BeneficiarioNome').val();
        const cpfCliente = $('#CPF').val();

        if (!cpfCliente) {
            ModalDialog("Atenção", "Preencha o CPF do cliente antes de adicionar os beneficiários.");
            return;
        }

        if (cpfCliente == cpf) {
            ModalDialog("Atenção", "CPF do beneficiário não pode ser igual ao CPF do cliente.");
            return;
        }

        if (!cpf || !nome) {
            ModalDialog("Atenção", "Preencha CPF e Nome do beneficiário.");
            return;
        }

        const linhaEditando = $('#tableBeneficiarios tr.linha-editando');
        if (linhaEditando.length > 0) {
            linhaEditando.remove();
        } else if (beneficiarioExiste(cpf)) {
            ModalDialog("Aviso", "Esse beneficiário já foi adicionado.");
            return;
        }

        $('#tableBeneficiarios').append(criarLinhaBeneficiario(cpf, nome));
        $('#BeneficiarioCPF, #BeneficiarioNome').val('');
    });

    $('#tableBeneficiarios').on('click', '.beneficiario-remove', function () {
        $(this).closest('tr').remove();
        $('#BeneficiarioCPF, #BeneficiarioNome').val('');
    });

    $('#tableBeneficiarios').on('click', '.beneficiario-edit', function () {
        const linha = $(this).closest('tr');

        $('#tableBeneficiarios tr').removeClass('linha-editando');

        linha.addClass('linha-editando');
        $('#BeneficiarioCPF').val(linha.find('.CPFBenefAdicionado').text());
        $('#BeneficiarioNome').val(linha.find('.NOMEBenefAdicionado').text());
    });

    $('#beneficiariosModal').on('hidden.bs.modal', function () {
        $('#BeneficiarioCPF, #BeneficiarioNome').val('');
        $('#tableBeneficiarios tr').removeClass('linha-editando');
    });
});
function obterDadosCliente($form) {
    return {
        Nome: $form.find("#Nome").val(),
        CEP: $form.find("#CEP").val(),
        Email: $form.find("#Email").val(),
        Sobrenome: $form.find("#Sobrenome").val(),
        Nacionalidade: $form.find("#Nacionalidade").val(),
        Estado: $form.find("#Estado").val(),
        Cidade: $form.find("#Cidade").val(),
        Logradouro: $form.find("#Logradouro").val(),
        Telefone: $form.find("#Telefone").val(),
        CPF: $form.find("#CPF").val().replaceAll(".", "").replace("-", "")
    };
}
function obterBeneficiarios() {
    const dados = [];
    $('#tableBeneficiarios tr').each(function () {
        const cpf = $(this).find('.CPFBenefAdicionado').text().replaceAll(".", "").replace("-", "");
        const nome = $(this).find('.NOMEBenefAdicionado').text();
        if (cpf && nome) {
            dados.push({ CPF: cpf, Nome: nome });
        }
    });
    return dados;
}
function beneficiarioExiste(cpf) {
    let existe = false;
    $('#tableBeneficiarios .CPFBenefAdicionado').each(function () {
        if ($(this).text().replaceAll(".", "").replace("-", "") === cpf.replaceAll(".", "").replace("-", "")) {
            existe = true;
        }
    });
    return existe;
}
function criarLinhaBeneficiario(cpf, nome) {
    return `
        <tr>
            <td class="CPFBenefAdicionado">${cpf}</td>
            <td class="NOMEBenefAdicionado">${nome}</td>
            <td>
                <button type="button" class="btn btn-primary beneficiario-edit">Editar</button>
                <button type="button" class="btn btn-primary beneficiario-remove">Excluir</button>
            </td>
        </tr>`;
}
function ModalDialog(titulo, texto) {
    var random = Math.random().toString().replace('.', '');
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
}
