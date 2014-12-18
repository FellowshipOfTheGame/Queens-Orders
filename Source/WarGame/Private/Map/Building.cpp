// Fill out your copyright notice in the Description page of Project Settings.

#include "WarGame.h"
#include "Building.h"

ABuilding::ABuilding(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	PrimaryActorTick.bStartWithTickEnabled = true;
	PrimaryActorTick.bCanEverTick = true;

	currentHP = 0.0f;
	built = false;
}


bool ABuilding::addHealth(float hp)
{
	currentHP += hp;

	if (currentHP >= maxHP)
	{
		currentHP = maxHP;

		if (!built)
		{
			built = true;
			return true;
		}
	}

	return false;
}


void ABuilding::ShowOptions()
{

}

void ABuilding::HideOptions()
{

}