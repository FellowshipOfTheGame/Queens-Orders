// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "GameFramework/PlayerController.h"

#include "BuildingSlot.h"

#include "QueenController.generated.h"

/**
 * 
 */
UCLASS()
class WARGAME_API AQueenController : public APlayerController
{
	GENERATED_BODY()

public:

	// Constructions
	UFUNCTION(BlueprintCallable, Category = "Buildings")
	void setSelectedBuildSlot(ABuildingSlot *slot);

	UFUNCTION(BlueprintCallable, BlueprintPure, Category = "Buildings")
	ABuildingSlot* getSelectedBuildSlot() const;

	// Actor interface
	virtual void BeginPlay() override;
	

private:
	ABuildingSlot *currentBuildingSlot;

};
