﻿using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.BL.AssetBL;
using MISA.QLTS.BL.AssetCategoryBL;
using MISA.QLTS.BL.BaseBL;
using MISA.QLTS.BL.BudgetPlaceBL;
using MISA.QLTS.BL.DepartmentBL;
using MISA.QLTS.BL.VoucherBL;
using MISA.QLTS.BL.VoucherBL;
using MISA.QLTS.BL.VoucherDetailBL;
using MISA.QLTS.DL.AssetCategoryDL;
using MISA.QLTS.DL.AssetDL;
using MISA.QLTS.DL.BaseDL;
using MISA.QLTS.DL.BudgetPlaceDL;
using MISA.QLTS.DL.Datacontext;
using MISA.QLTS.DL.DepartmentDL;
using MISA.QLTS.DL.VoucherDetailDL;
using MISA.QLTS.DL.VoucherDL;

var builder = WebApplication.CreateBuilder(args);

DataBaseContext.connectionString = builder.Configuration.GetConnectionString("MySql");


// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        }
        );

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(p => p.AddPolicy("MyCors", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

//Tắt validate mặc định
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddScoped(typeof(IBaseBL<>), typeof(BaseBL<>));
builder.Services.AddScoped(typeof(IBaseDL<>), typeof(BaseDL<>));

builder.Services.AddScoped<IAssetBL, AssetBL>();
builder.Services.AddScoped<IAssetDL, AssetDL>();

builder.Services.AddScoped<IDepartmentBL, DepartmentBL>();
builder.Services.AddScoped<IDepartmentDL, DepartmentDL>();

builder.Services.AddScoped<IAssetCategoryBL, AssetCategoryBL>();
builder.Services.AddScoped<IAssetCategoryDL, AssetCategoryDL>();

builder.Services.AddScoped<IVoucherBL, VoucherBL>();
builder.Services.AddScoped<IVoucherDL, VoucherDL>();

builder.Services.AddScoped<IVoucherDetailBL, VoucherDetailBL>();
builder.Services.AddScoped<IVoucherDetailDL, VoucherDetailDL>();

builder.Services.AddScoped<IBudgetPlaceBL, BudgetPlaceBL>();
builder.Services.AddScoped<IBudgetPlaceDL, BudgetPlaceDL>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyCors");

app.UseAuthorization();

app.MapControllers();

app.Run();
