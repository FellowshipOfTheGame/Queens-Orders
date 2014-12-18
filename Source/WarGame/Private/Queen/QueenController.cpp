// Fill out your copyright notice in the Description page of Project Settings.

#include "WarGame.h"
#include "QueenController.h"

void AQueenController::BeginPlay()
{
	Super::BeginPlay();

	bShowMouseCursor = true;
	bEnableClickEvents = true;
	bEnableMouseOverEvents = true;
}

void AQueenController::setSelectedBuildSlot(ABuildingSlot *slot)
{
	if (currentBuildingSlot == slot)
		return;

	// Hide options for that object
	if (currentBuildingSlot){
		currentBuildingSlot->HideOptions();
	}

	currentBuildingSlot = slot;

	// Show options for selected object
	if (currentBuildingSlot){
		currentBuildingSlot->ShowOptions();
	}
}

ABuildingSlot* AQueenController::getSelectedBuildSlot() const
{
	return currentBuildingSlot;
}